using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class SecurityGuard : RoleBase<SecurityGuard>
{
    public static Color NameColor = new Color32(195, 178, 95, byte.MaxValue);

    public static CustomButton SecurityGuardButton;
    public static CustomButton SecurityGuardCamButton;
    public static TMP_Text SecurityGuardButtonScrewsText;
    public static TMP_Text SecurityGuardChargesText;
    private static SystemConsole _survPanelConsole;
    private static SystemConsole _survConsole;
    private static SystemConsole _taskCamsConsole;
    private static SystemConsole _doorLogConsole;
    public int Charges = 1;
    public Minigame Minigame;
    public int PlacedCameras;

    public int RechargedTasks = 3;
    public int RemainingScrews = 7;
    public Vent VentTarget;

    public SecurityGuard()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.SecurityGuard;
        PlacedCameras = 0;
        RemainingScrews = TotalScrews;
        Charges = CamMaxCharges;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.SecurityGuardCooldown.GetFloat();
    }

    public static int TotalScrews
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardTotalScrews.GetFloat());
    }

    public static int CamPrice
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamPrice.GetFloat());
    }

    public static int VentPrice
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardVentPrice.GetFloat());
    }

    public static float CamDuration
    {
        get => CustomOptionHolder.SecurityGuardCamDuration.GetFloat();
    }

    public static int CamMaxCharges
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamMaxCharges.GetFloat());
    }

    public static int CamRechargeTasksNumber
    {
        get => Mathf.RoundToInt(CustomOptionHolder.SecurityGuardCamRechargeTasksNumber.GetFloat());
    }

    public static bool NoMove
    {
        get => CustomOptionHolder.SecurityGuardNoMove.GetBool();
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }

    public override void FixedUpdate()
    {
        if (!PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard) || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

        Vent target = null;
        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        var closestDistance = float.MaxValue;
        for (var i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
        {
            var vent = MapUtilities.CachedShipStatus.AllVents[i];
            if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
            var distance = Vector2.Distance(vent.transform.position, truePosition);
            if (distance <= vent.UsableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = vent;
            }
        }

        VentTarget = target;

        if (Player.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
        if (playerCompleted == RechargedTasks)
        {
            RechargedTasks += CamRechargeTasksNumber;
            if (CamMaxCharges > Charges) Charges++;
        }
    }

    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }

    public static void MakeButtons(HudManager hm)
    {
        SecurityGuardButton = new(() =>
        {
            if (Local.VentTarget != null)
            {
                // Seal vent
                using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.SealVent);
                sender.WritePacked(Local.VentTarget.Id);
                sender.Write(Local.Player.PlayerId);
                RPCProcedure.SealVent(Local.VentTarget.Id, Local.Player.PlayerId);
                Local.VentTarget = null;
            }
            else if (Helpers.GetOption(ByteOptionNames.MapId) != 1 && MapSettings.CouldUseCameras && !SubmergedCompatibility.IsSubmerged)
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

                using (var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.PlaceCamera))
                {
                    sender.Write(pos.x);
                    sender.Write(pos.y);
                    sender.Write(roomId);
                    sender.Write(Local.Player.PlayerId);
                }

                RPCProcedure.PlaceCamera(pos.x, pos.y, roomId, Local.Player.PlayerId);
            }

            SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer;
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard) && PlayerControl.LocalPlayer.IsAlive() && Local.RemainingScrews >= Mathf.Min(VentPrice, CamPrice); }, () =>
        {
            if (Local.VentTarget == null && Helpers.GetOption(ByteOptionNames.MapId) != 1 && !SubmergedCompatibility.IsSubmerged)
            {
                SecurityGuardButton.ButtonText = Tr.Get(TrKey.PlaceCameraText);
                SecurityGuardButton.Sprite = AssetLoader.PlaceCameraButton;
            }
            else
            {
                SecurityGuardButton.ButtonText = Tr.Get(TrKey.CloseVentText);
                SecurityGuardButton.Sprite = AssetLoader.CloseVentButton;
            }

            SecurityGuardButtonScrewsText?.text = string.Format(Tr.Get(TrKey.SecurityGuardScrews), Local.RemainingScrews);

            return Local.VentTarget != null ? Local.RemainingScrews >= VentPrice && PlayerControl.LocalPlayer.CanMove : Helpers.GetOption(ByteOptionNames.MapId) != 1 && !SubmergedCompatibility.IsSubmerged && MapSettings.CouldUseCameras && Local.RemainingScrews >= CamPrice && PlayerControl.LocalPlayer.CanMove;
        }, () => { SecurityGuardButton.Timer = SecurityGuardButton.MaxTimer; }, AssetLoader.PlaceCameraButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, false, Tr.Get(TrKey.PlaceCameraText));

        SecurityGuardButtonScrewsText = GameObject.Instantiate(SecurityGuardButton.ActionButton.cooldownTimerText, SecurityGuardButton.ActionButton.cooldownTimerText.transform.parent);
        SecurityGuardButtonScrewsText.text = "";
        SecurityGuardButtonScrewsText.enableWordWrapping = false;
        SecurityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
        SecurityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        SecurityGuardCamButton = new(() =>
        {
            if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
            {
                if (Local.Minigame == null)
                {
                    var mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
                    SystemConsole targetConsole = null;

                    if (mapId is 0 or 3)
                    {
                        if (_survConsole == null)
                        {
                            var consoles = Object.FindObjectsOfType<SystemConsole>();
                            for (var i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("SurvConsole"))
                                {
                                    _survConsole = consoles[i];
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
                            var consoles = Object.FindObjectsOfType<SystemConsole>();
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
                            var consoles = Object.FindObjectsOfType<SystemConsole>();
                            for (var i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("Surv_Panel"))
                                {
                                    _survPanelConsole = consoles[i];
                                    break;
                                }
                            }
                        }

                        targetConsole = _survPanelConsole;
                    }

                    if (targetConsole == null || Camera.main == null) return;
                    Local.Minigame = Object.Instantiate(targetConsole.MinigamePrefab, Camera.main.transform, false);
                }

                Local.Minigame.transform.SetParent(Camera.main.transform, false);
                Local.Minigame.transform.localPosition = new(0.0f, 0.0f, -50f);
                Local.Minigame.Begin(null);
            }
            else
            {
                if (Local.Minigame == null)
                {
                    if (_doorLogConsole == null)
                    {
                        var consoles = Object.FindObjectsOfType<SystemConsole>();
                        for (var i = 0; i < consoles.Length; i++)
                        {
                            if (consoles[i].gameObject.name.Contains("SurvLogConsole"))
                            {
                                _doorLogConsole = consoles[i];
                                break;
                            }
                        }
                    }

                    if (_doorLogConsole == null || Camera.main == null) return;
                    Local.Minigame = Object.Instantiate(_doorLogConsole.MinigamePrefab, Camera.main.transform, false);
                }

                Local.Minigame.transform.SetParent(Camera.main.transform, false);
                Local.Minigame.transform.localPosition = new(0.0f, 0.0f, -50f);
                Local.Minigame.Begin(null);
            }

            Local.Charges--;

            if (NoMove) PlayerControl.LocalPlayer.moveable = false;
            PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.SecurityGuard) && PlayerControl.LocalPlayer?.Data?.IsDead == false && Local.RemainingScrews < Mathf.Min(VentPrice, CamPrice) && !SubmergedCompatibility.IsSubmerged; }, () =>
        {
            SecurityGuardChargesText?.text = string.Format(Tr.Get(TrKey.HackerChargesText), Local.Charges, CamMaxCharges);
            SecurityGuardCamButton.ActionButton.graphic.sprite = Helpers.IsMiraHq ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
            SecurityGuardCamButton.ActionButton.OverrideText(Helpers.IsMiraHq ? TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) : TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));
            return PlayerControl.LocalPlayer.CanMove && Local.Charges > 0;
        }, () =>
        {
            SecurityGuardCamButton.Timer = SecurityGuardCamButton.MaxTimer;
            SecurityGuardCamButton.IsEffectActive = false;
            SecurityGuardCamButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, 0f, () =>
        {
            SecurityGuardCamButton.Timer = SecurityGuardCamButton.MaxTimer;
            if (Minigame.Instance) Local.Minigame.ForceClose();
            PlayerControl.LocalPlayer.moveable = true;
        }, false, Helpers.IsMiraHq ? TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) : TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));

        // Security Guard cam button charges
        SecurityGuardChargesText = Object.Instantiate(SecurityGuardCamButton.ActionButton.cooldownTimerText, SecurityGuardCamButton.ActionButton.cooldownTimerText.transform.parent);
        SecurityGuardChargesText.text = "";
        SecurityGuardChargesText.enableWordWrapping = false;
        SecurityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        SecurityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    public static void SetButtonCooldowns()
    {
        SecurityGuardButton.MaxTimer = Cooldown;
        SecurityGuardCamButton.MaxTimer = Cooldown;
        SecurityGuardCamButton.EffectDuration = CamDuration;
    }

    public static Sprite GetAnimatedVentSealedSprite()
    {
        if (SubmergedCompatibility.IsSubmerged) return AssetLoader.AnimatedVentSealedSubmerged;
        return AssetLoader.AnimatedVentSealed;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}
