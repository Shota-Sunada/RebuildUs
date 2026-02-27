namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
internal class SecurityGuard : SingleRoleBase<SecurityGuard>
{
    internal static Color NameColor = new Color32(195, 178, 95, byte.MaxValue);

    private static CustomButton _securityGuardButton;
    internal static CustomButton SecurityGuardCamButton;
    private static TMP_Text _securityGuardButtonScrewsText;
    private static TMP_Text _securityGuardChargesText;
    private static SystemConsole _survPanelConsole;
    private static SystemConsole _survConsole;
    private static SystemConsole _taskCamsConsole;
    private static SystemConsole _doorLogConsole;
    private int _charges = 1;
    private Minigame _minigame;

    private int _rechargedTasks = 3;
    private Vent _ventTarget;
    internal int PlacedCameras;
    internal int RemainingScrews = 7;

    public SecurityGuard()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.SecurityGuard;
        PlacedCameras = 0;
        RemainingScrews = TotalScrews;
        _charges = CamMaxCharges;
    }

    internal override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    internal static float Cooldown
    {
        get => CustomOptionHolder.SecurityGuardCooldown.GetFloat();
    }

    internal static int TotalScrews
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardTotalScrews.GetFloat());
    }

    internal static int CamPrice
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamPrice.GetFloat());
    }

    internal static int VentPrice
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardVentPrice.GetFloat());
    }

    internal static float CamDuration
    {
        get => CustomOptionHolder.SecurityGuardCamDuration.GetFloat();
    }

    internal static int CamMaxCharges
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamMaxCharges.GetFloat());
    }

    internal static int CamRechargeTasksNumber
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamRechargeTasksNumber.GetFloat());
    }

    internal static bool NoMove
    {
        get => CustomOptionHolder.SecurityGuardNoMove.GetBool();
    }

    internal override void OnMeetingStart() { }
    internal override void OnMeetingEnd() { }
    internal override void OnIntroEnd() { }

    internal override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard)
            || MapUtilities.CachedShipStatus == null
            || MapUtilities.CachedShipStatus.AllVents == null)
        {
            return;
        }

        Vent target = null;
        Vector2 truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        float closestDistance = float.MaxValue;
        for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
        {
            Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
            if (vent.gameObject.name.StartsWith("JackInTheBoxVent_")
                || vent.gameObject.name.StartsWith("SealedVent_")
                || vent.gameObject.name.StartsWith("FutureSealedVent_"))
            {
                continue;
            }
            float distance = Vector2.Distance(vent.transform.position, truePosition);
            if (distance <= vent.UsableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = vent;
            }
        }

        _ventTarget = target;

        if (Player.Data.IsDead)
        {
            return;
        }
        (int playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
        if (playerCompleted == _rechargedTasks)
        {
            _rechargedTasks += CamRechargeTasksNumber;
            if (CamMaxCharges > _charges)
            {
                _charges++;
            }
        }
    }

    internal override void OnKill(PlayerControl target) { }
    internal override void OnDeath(PlayerControl killer = null) { }
    internal override void OnFinishShipStatusBegin() { }
    internal override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    internal static void MakeButtons(HudManager hm)
    {
        _securityGuardButton = new(() =>
            {
                if (Local._ventTarget != null)
                {
                    // Seal vent
                    using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.SealVent);
                    sender.WritePacked(Local._ventTarget.Id);
                    RPCProcedure.SealVent(Local._ventTarget.Id);
                    Local._ventTarget = null;
                }
                else if (Helpers.GetOption(ByteOptionNames.MapId) != 1 && MapSettings.CouldUseCameras && !SubmergedCompatibility.IsSubmerged)
                {
                    // Place camera if there's no vent and it's not MiraHQ
                    Vector3 pos = PlayerControl.LocalPlayer.transform.position;

                    byte roomId;
                    try
                    {
                        roomId = (byte)FastDestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
                    }
                    catch
                    {
                        roomId = 255;
                    }

                    using (RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceCamera))
                    {
                        sender.Write(pos.x);
                        sender.Write(pos.y);
                        sender.Write(roomId);
                    }

                    RPCProcedure.PlaceCamera(pos.x, pos.y, roomId);
                }

                _securityGuardButton.Timer = _securityGuardButton.MaxTimer;
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard)
                       && PlayerControl.LocalPlayer.IsAlive()
                       && Local.RemainingScrews >= Mathf.Min(VentPrice, CamPrice);
            },
            () =>
            {
                if (Local._ventTarget == null && Helpers.GetOption(ByteOptionNames.MapId) != 1 && !SubmergedCompatibility.IsSubmerged)
                {
                    _securityGuardButton.ButtonText = Tr.Get(TrKey.PlaceCameraText);
                    _securityGuardButton.Sprite = AssetLoader.PlaceCameraButton;
                }
                else
                {
                    _securityGuardButton.ButtonText = Tr.Get(TrKey.CloseVentText);
                    _securityGuardButton.Sprite = AssetLoader.CloseVentButton;
                }

                _securityGuardButtonScrewsText?.text = string.Format(Tr.Get(TrKey.SecurityGuardScrews), Local.RemainingScrews);

                return Local._ventTarget != null
                    ? Local.RemainingScrews >= VentPrice && PlayerControl.LocalPlayer.CanMove
                    : Helpers.GetOption(ByteOptionNames.MapId) != 1
                      && !SubmergedCompatibility.IsSubmerged
                      && MapSettings.CouldUseCameras
                      && Local.RemainingScrews >= CamPrice
                      && PlayerControl.LocalPlayer.CanMove;
            },
            () =>
            {
                _securityGuardButton.Timer = _securityGuardButton.MaxTimer;
            },
            AssetLoader.PlaceCameraButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            false,
            Tr.Get(TrKey.PlaceCameraText));

        _securityGuardButtonScrewsText = UnityObject.Instantiate(_securityGuardButton.ActionButton.cooldownTimerText,
            _securityGuardButton.ActionButton.cooldownTimerText.transform.parent);
        _securityGuardButtonScrewsText.text = "";
        _securityGuardButtonScrewsText.enableWordWrapping = false;
        _securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
        _securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        SecurityGuardCamButton = new(() =>
            {
                if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
                {
                    if (Local._minigame == null)
                    {
                        byte mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
                        SystemConsole targetConsole = null;

                        if (mapId is 0 or 3)
                        {
                            if (_survConsole == null)
                            {
                                Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                foreach (SystemConsole t in consoles)
                                {
                                    if (t.gameObject.name.Contains("SurvConsole"))
                                    {
                                        _survConsole = t;
                                        break;
                                    }
                                }
                            }

                            targetConsole = _survConsole;
                        }
                        else if (mapId == 4)
                        {
                            if (_taskCamsConsole == null)
                            {
                                Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                for (int i = 0; i < consoles.Length; i++)
                                {
                                    if (consoles[i].gameObject.name.Contains("task_cams"))
                                    {
                                        _taskCamsConsole = consoles[i];
                                        break;
                                    }
                                }
                            }

                            targetConsole = _taskCamsConsole;
                        }
                        else
                        {
                            if (_survPanelConsole == null)
                            {
                                Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                foreach (SystemConsole t in consoles)
                                {
                                    if (t.gameObject.name.Contains("Surv_Panel"))
                                    {
                                        _survPanelConsole = t;
                                        break;
                                    }
                                }
                            }

                            targetConsole = _survPanelConsole;
                        }

                        if (targetConsole == null || Camera.main == null)
                        {
                            return;
                        }
                        Local._minigame = UnityObject.Instantiate(targetConsole.MinigamePrefab, Camera.main.transform, false);
                    }

                    if (Camera.main != null)
                    {
                        Local._minigame.transform.SetParent(Camera.main.transform, false);
                    }
                    Local._minigame.transform.localPosition = new(0.0f, 0.0f, -50f);
                    Local._minigame.Begin(null);
                }
                else
                {
                    if (Local._minigame == null)
                    {
                        if (_doorLogConsole == null)
                        {
                            Il2CppArrayBase<SystemConsole> consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                            for (int i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("SurvLogConsole"))
                                {
                                    _doorLogConsole = consoles[i];
                                    break;
                                }
                            }
                        }

                        if (_doorLogConsole == null || Camera.main == null)
                        {
                            return;
                        }
                        Local._minigame = UnityObject.Instantiate(_doorLogConsole.MinigamePrefab, Camera.main.transform, false);
                    }

                    if (Camera.main != null)
                    {
                        Local._minigame.transform.SetParent(Camera.main.transform, false);
                    }
                    Local._minigame.transform.localPosition = new(0.0f, 0.0f, -50f);
                    Local._minigame.Begin(null);
                }

                Local._charges--;

                if (NoMove)
                {
                    PlayerControl.LocalPlayer.moveable = false;
                }
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
            },
            () =>
            {
                return PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard)
                       && PlayerControl.LocalPlayer?.Data?.IsDead == false
                       && Local.RemainingScrews < Mathf.Min(VentPrice, CamPrice)
                       && !SubmergedCompatibility.IsSubmerged;
            },
            () =>
            {
                _securityGuardChargesText?.text = string.Format(Tr.Get(TrKey.HackerChargesText), Local._charges, CamMaxCharges);
                SecurityGuardCamButton.ActionButton.graphic.sprite = Helpers.IsMiraHq
                    ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image
                    : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
                SecurityGuardCamButton.ActionButton.OverrideText(Helpers.IsMiraHq
                    ? TranslationController.Instance.GetString(StringNames.SecurityLogsSystem)
                    : TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));
                return PlayerControl.LocalPlayer.CanMove && Local._charges > 0;
            },
            () =>
            {
                SecurityGuardCamButton.Timer = SecurityGuardCamButton.MaxTimer;
                SecurityGuardCamButton.IsEffectActive = false;
                SecurityGuardCamButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            true,
            0f,
            () =>
            {
                SecurityGuardCamButton.Timer = SecurityGuardCamButton.MaxTimer;
                if (Minigame.Instance)
                {
                    Local._minigame.ForceClose();
                }

                PlayerControl.LocalPlayer.moveable = true;
            },
            false,
            Helpers.IsMiraHq
                ? TranslationController.Instance.GetString(StringNames.SecurityLogsSystem)
                : TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));

        // Security Guard cam button charges
        _securityGuardChargesText = UnityObject.Instantiate(SecurityGuardCamButton.ActionButton.cooldownTimerText,
            SecurityGuardCamButton.ActionButton.cooldownTimerText.transform.parent);
        _securityGuardChargesText.text = "";
        _securityGuardChargesText.enableWordWrapping = false;
        _securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        _securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    internal static void SetButtonCooldowns()
    {
        _securityGuardButton.MaxTimer = Cooldown;
        SecurityGuardCamButton.MaxTimer = Cooldown;
        SecurityGuardCamButton.EffectDuration = CamDuration;
    }

    internal static Sprite GetAnimatedVentSealedSprite()
    {
        if (SubmergedCompatibility.IsSubmerged)
        {
            return AssetLoader.AnimatedVentSealedSubmerged;
        }
        return AssetLoader.AnimatedVentSealed;
    }

    // write functions here

    internal static void Clear()
    {
        ModRoleManager.RemoveRole(Instance);
        Instance = null;
    }
}