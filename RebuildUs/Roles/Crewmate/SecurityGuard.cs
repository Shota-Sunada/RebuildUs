using PowerTools;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
[RegisterRole(RoleType.SecurityGuard, RoleTeam.Crewmate, typeof(SingleRoleBase<SecurityGuard>), nameof(CustomOptionHolder.SecurityGuardSpawnRate))]
internal class SecurityGuard : SingleRoleBase<SecurityGuard>
{
    internal static Color Color = new Color32(195, 178, 95, byte.MaxValue);

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

    [CustomEvent(CustomEventType.FixedUpdate)]
    internal void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard)
            || MapUtilities.CachedShipStatus == null
            || MapUtilities.CachedShipStatus.AllVents == null)
        {
            return;
        }

        Vent target = null;
        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        var closestDistance = float.MaxValue;
        for (var i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
        {
            var vent = MapUtilities.CachedShipStatus.AllVents[i];
            if (vent.gameObject.name.StartsWith("JackInTheBoxVent_")
                || vent.gameObject.name.StartsWith("SealedVent_")
                || vent.gameObject.name.StartsWith("FutureSealedVent_"))
            {
                continue;
            }
            var distance = Vector2.Distance(vent.transform.position, truePosition);
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
        (var playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
        if (playerCompleted == _rechargedTasks)
        {
            _rechargedTasks += CamRechargeTasksNumber;
            if (CamMaxCharges > _charges)
            {
                _charges++;
            }
        }
    }

    [RegisterCustomButton]
    internal static void MakeButtons(HudManager hm)
    {
        _securityGuardButton = new(() =>
            {
                if (Local._ventTarget != null)
                {
                    // Seal vent
                    SealVent(PlayerControl.LocalPlayer, Local._ventTarget.Id);
                    Local._ventTarget = null;
                }
                else if (ByteOptionNames.MapId.Get() != 1 && MapSettings.CouldUseCameras && !SubmergedCompatibility.IsSubmerged)
                {
                    // Place camera if there's no vent and it's not MiraHQ
                    var pos = PlayerControl.LocalPlayer.transform.position;

                    byte roomId;
                    try
                    {
                        roomId = (byte)FastDestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
                    }
                    catch
                    {
                        roomId = 255;
                    }

                    PlaceCamera(PlayerControl.LocalPlayer, pos.x, pos.y, roomId);
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
                if (Local._ventTarget == null && ByteOptionNames.MapId.Get() != 1 && !SubmergedCompatibility.IsSubmerged)
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
                    : ByteOptionNames.MapId.Get() != 1
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
                if (ByteOptionNames.MapId.Get() != 1)
                {
                    if (Local._minigame == null)
                    {
                        var mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
                        SystemConsole targetConsole = null;

                        if (mapId is 0 or 3)
                        {
                            if (_survConsole == null)
                            {
                                var consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                foreach (var t in consoles)
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
                                var consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                for (var i = 0; i < consoles.Length; i++)
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
                                var consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                                foreach (var t in consoles)
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
                            var consoles = UnityObject.FindObjectsOfType<SystemConsole>();
                            for (var i = 0; i < consoles.Length; i++)
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
                    ? FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.SecurityLogsSystem)
                    : FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.SecurityCamsSystem));
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
                ? FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.SecurityLogsSystem)
                : FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.SecurityCamsSystem));

        // Security Guard cam button charges
        _securityGuardChargesText = UnityObject.Instantiate(SecurityGuardCamButton.ActionButton.cooldownTimerText,
            SecurityGuardCamButton.ActionButton.cooldownTimerText.transform.parent);
        _securityGuardChargesText.text = "";
        _securityGuardChargesText.enableWordWrapping = false;
        _securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        _securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    [RegisterCustomButton]
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

    [MethodRpc((uint)CustomRPC.PlaceCamera)]
    internal static void PlaceCamera(PlayerControl sender, float x, float y, byte roomId)
    {
        var sg = Instance;

        var referenceCamera = UnityObject.FindObjectOfType<SurvCamera>();
        if (referenceCamera == null)
        {
            return;
        }

        sg.RemainingScrews -= CamPrice;
        sg.PlacedCameras++;

        Vector3 position = new(x, y);

        var roomType = (SystemTypes)roomId;

        var camera = UnityObject.Instantiate(referenceCamera);
        camera.transform.position = new(position.x, position.y, referenceCamera.transform.position.z - 1f);
        camera.CamName = $"Security Camera {sg.PlacedCameras}";
        camera.Offset = new(0f, 0f, camera.Offset.z);

        camera.NewName = roomType switch
        {
            SystemTypes.Hallway => StringNames.Hallway,
            SystemTypes.Storage => StringNames.Storage,
            SystemTypes.Cafeteria => StringNames.Cafeteria,
            SystemTypes.Reactor => StringNames.Reactor,
            SystemTypes.UpperEngine => StringNames.UpperEngine,
            SystemTypes.Nav => StringNames.Nav,
            SystemTypes.Admin => StringNames.Admin,
            SystemTypes.Electrical => StringNames.Electrical,
            SystemTypes.LifeSupp => StringNames.LifeSupp,
            SystemTypes.Shields => StringNames.Shields,
            SystemTypes.MedBay => StringNames.MedBay,
            SystemTypes.Security => StringNames.Security,
            SystemTypes.Weapons => StringNames.Weapons,
            SystemTypes.LowerEngine => StringNames.LowerEngine,
            SystemTypes.Comms => StringNames.Comms,
            SystemTypes.Decontamination => StringNames.Decontamination,
            SystemTypes.Launchpad => StringNames.Launchpad,
            SystemTypes.LockerRoom => StringNames.LockerRoom,
            SystemTypes.Laboratory => StringNames.Laboratory,
            SystemTypes.Balcony => StringNames.Balcony,
            SystemTypes.Office => StringNames.Office,
            SystemTypes.Greenhouse => StringNames.Greenhouse,
            SystemTypes.Dropship => StringNames.Dropship,
            SystemTypes.Decontamination2 => StringNames.Decontamination2,
            SystemTypes.Outside => StringNames.Outside,
            SystemTypes.Specimens => StringNames.Specimens,
            SystemTypes.BoilerRoom => StringNames.BoilerRoom,
            SystemTypes.VaultRoom => StringNames.VaultRoom,
            SystemTypes.Cockpit => StringNames.Cockpit,
            SystemTypes.Armory => StringNames.Armory,
            SystemTypes.Kitchen => StringNames.Kitchen,
            SystemTypes.ViewingDeck => StringNames.ViewingDeck,
            SystemTypes.HallOfPortraits => StringNames.HallOfPortraits,
            SystemTypes.CargoBay => StringNames.CargoBay,
            SystemTypes.Ventilation => StringNames.Ventilation,
            SystemTypes.Showers => StringNames.Showers,
            SystemTypes.Engine => StringNames.Engine,
            SystemTypes.Brig => StringNames.Brig,
            SystemTypes.MeetingRoom => StringNames.MeetingRoom,
            SystemTypes.Records => StringNames.Records,
            SystemTypes.Lounge => StringNames.Lounge,
            SystemTypes.GapRoom => StringNames.GapRoom,
            SystemTypes.MainHall => StringNames.MainHall,
            SystemTypes.Medical => StringNames.Medical,
            _ => StringNames.ExitButton,
        };
        if (ByteOptionNames.MapId.Get() is 2 or 4)
        {
            camera.transform.localRotation = new(0, 0, 1, 1);
        }

        if (PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard))
        {
            camera.gameObject.SetActive(true);
            camera.gameObject.GetComponent<SpriteRenderer>().color = new(1f, 1f, 1f, 0.5f);
        }
        else
        {
            camera.gameObject.SetActive(false);
        }

        MapSettings.CamerasToAdd.Add(camera);
    }

    [MethodRpc((uint)CustomRPC.SealVent)]
    internal static void SealVent(PlayerControl sender, int ventId)
    {
        var sg = Instance;

        Vent vent = null;
        var allVents = MapUtilities.CachedShipStatus.AllVents;
        foreach (var v in allVents)
        {
            if (v == null || v.Id != ventId)
            {
                continue;
            }
            vent = v;
            break;
        }

        if (vent == null)
        {
            return;
        }

        sg.RemainingScrews -= VentPrice;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard))
        {
            var animator = vent.GetComponent<SpriteAnim>();
            animator?.Stop();
            vent.EnterVentAnim = vent.ExitVentAnim = null;
            vent.myRend.sprite = animator == null ? AssetLoader.StaticVentSealed : AssetLoader.AnimatedVentSealed;
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 0)
            {
                vent.myRend.sprite = AssetLoader.CentralUpperBlocked;
            }
            if (SubmergedCompatibility.IsSubmerged && vent.Id == 14)
            {
                vent.myRend.sprite = AssetLoader.CentralLowerBlocked;
            }
            vent.myRend.color = new(1f, 1f, 1f, 0.5f);
            vent.name = "FutureSealedVent_" + vent.name;
        }

        MapSettings.VentsToSeal.Add(vent);
    }
}