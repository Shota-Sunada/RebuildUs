using Sentry.Unity.NativeUtils;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class SecurityGuard : RoleBase<SecurityGuard>
{
    public static Color RoleColor = new Color32(195, 178, 95, byte.MaxValue);
    public static CustomButton securityGuardButton;
    public static CustomButton securityGuardCamButton;
    public static TMP_Text securityGuardButtonScrewsText;
    public static TMP_Text securityGuardChargesText;
    public Vent ventTarget = null;
    public Minigame minigame = null;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.securityGuardCooldown.GetFloat(); } }
    public static int totalScrews { get { return Mathf.RoundToInt(CustomOptionHolder.securityGuardTotalScrews.GetFloat()); } }
    public static int camPrice { get { return Mathf.RoundToInt(CustomOptionHolder.securityGuardCamPrice.GetFloat()); } }
    public static int ventPrice { get { return Mathf.RoundToInt(CustomOptionHolder.securityGuardVentPrice.GetFloat()); } }
    public static float camDuration { get { return CustomOptionHolder.securityGuardCamDuration.GetFloat(); } }
    public static int camMaxCharges { get { return Mathf.RoundToInt(CustomOptionHolder.securityGuardCamMaxCharges.GetFloat()); } }
    public static int camRechargeTasksNumber { get { return Mathf.RoundToInt(CustomOptionHolder.securityGuardCamRechargeTasksNumber.GetFloat()); } }
    public static bool noMove { get { return CustomOptionHolder.securityGuardNoMove.GetBool(); } }

    public int rechargedTasks = 3;
    public int charges = 1;
    public int placedCameras = 0;
    public int remainingScrews = 7;

    public SecurityGuard()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.SecurityGuard;
        placedCameras = 0;
        charges = camMaxCharges;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.SecurityGuard) || MapUtilities.CachedShipStatus == null || MapUtilities.CachedShipStatus.AllVents == null) return;

        Vent target = null;
        Vector2 truePosition = CachedPlayer.LocalPlayer.PlayerControl.GetTruePosition();
        float closestDistance = float.MaxValue;
        for (int i = 0; i < MapUtilities.CachedShipStatus.AllVents.Length; i++)
        {
            Vent vent = MapUtilities.CachedShipStatus.AllVents[i];
            if (vent.gameObject.name.StartsWith("JackInTheBoxVent_") || vent.gameObject.name.StartsWith("SealedVent_") || vent.gameObject.name.StartsWith("FutureSealedVent_")) continue;
            float distance = Vector2.Distance(vent.transform.position, truePosition);
            if (distance <= vent.UsableDistance && distance < closestDistance)
            {
                closestDistance = distance;
                target = vent;
            }
        }
        ventTarget = target;

        if (Player.Data.IsDead) return;
        var (playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
        if (playerCompleted == rechargedTasks)
        {
            rechargedTasks += camRechargeTasksNumber;
            if (camMaxCharges > charges) charges++;
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        securityGuardButton = new CustomButton(
                () =>
                {
                    if (ventTarget != null)
                    {
                        // Seal vent
                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.SealVent);
                        sender.WritePacked(ventTarget.Id);
                        sender.Write(Player.PlayerId);
                        RPCProcedure.sealVent(ventTarget.Id, Player.PlayerId);
                        ventTarget = null;
                    }
                    else if (Helpers.GetOption(ByteOptionNames.MapId) != 1 && ModMapOptions.couldUseCameras && !SubmergedCompatibility.IsSubmerged)
                    {
                        // Place camera if there's no vent and it's not MiraHQ
                        var pos = CachedPlayer.LocalPlayer.PlayerControl.transform.position;
                        byte[] buff = new byte[sizeof(float) * 2];
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.x), 0, buff, 0 * sizeof(float), sizeof(float));
                        Buffer.BlockCopy(BitConverter.GetBytes(pos.y), 0, buff, 1 * sizeof(float), sizeof(float));

                        byte roomId;
                        try
                        {
                            roomId = (byte)FastDestroyableSingleton<HudManager>.Instance.roomTracker.LastRoom.RoomId;
                        }
                        catch
                        {
                            roomId = 255;
                        }

                        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.PlaceCamera);
                        sender.WriteBytesAndSize(buff);
                        sender.Write(roomId);
                        sender.Write(Player.PlayerId);
                        RPCProcedure.placeCamera(buff, roomId, Player.PlayerId);
                    }
                    securityGuardButton.Timer = securityGuardButton.MaxTimer;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.SecurityGuard) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && Local.remainingScrews >= Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice); },
                () =>
                {
                    if (Local.ventTarget == null && Helpers.GetOption(ByteOptionNames.MapId) != 1 && SubmergedCompatibility.IsSubmerged)
                    {
                        securityGuardButton.ButtonText = Tr.Get("PlaceCameraText");
                        securityGuardButton.Sprite = AssetLoader.PlaceCameraButton;
                    }
                    else
                    {
                        securityGuardButton.ButtonText = Tr.Get("CloseVentText");
                        securityGuardButton.Sprite = AssetLoader.CloseVentButton;
                    }
                    securityGuardButtonScrewsText?.text = String.Format(Tr.Get("securityGuardScrews"), Local.remainingScrews);

                    return Local.ventTarget != null
                        ? Local.remainingScrews >= SecurityGuard.ventPrice && CachedPlayer.LocalPlayer.PlayerControl.CanMove
                        : Helpers.GetOption(ByteOptionNames.MapId) != 1 && SubmergedCompatibility.IsSubmerged && ModMapOptions.couldUseCameras && Local.remainingScrews >= SecurityGuard.camPrice && CachedPlayer.LocalPlayer.PlayerControl.CanMove;
                },
                () => { securityGuardButton.Timer = securityGuardButton.MaxTimer; },
                AssetLoader.PlaceCameraButton,
                new Vector3(-1.8f, -0.06f, 0),
                hm,
                hm.UseButton,
                KeyCode.F
            )
        {
            ButtonText = Tr.Get("PlaceCameraText")
        };

        securityGuardButtonScrewsText = GameObject.Instantiate(securityGuardButton.ActionButton.cooldownTimerText, securityGuardButton.ActionButton.cooldownTimerText.transform.parent);
        securityGuardButtonScrewsText.text = "";
        securityGuardButtonScrewsText.enableWordWrapping = false;
        securityGuardButtonScrewsText.transform.localScale = Vector3.one * 0.5f;
        securityGuardButtonScrewsText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        securityGuardCamButton = new CustomButton(
            () =>
            {
                if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
                {
                    if (Local.minigame == null)
                    {
                        byte mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
                        var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("Surv_Panel"));
                        if (mapId is 0 or 3) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvConsole"));
                        else if (mapId == 4) e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("task_cams"));
                        if (e == null || Camera.main == null) return;
                        Local.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }
                    Local.minigame.transform.SetParent(Camera.main.transform, false);
                    Local.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Local.minigame.Begin(null);
                }
                else
                {
                    if (Local.minigame == null)
                    {
                        var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                        if (e == null || Camera.main == null) return;
                        Local.minigame = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                    }
                    Local.minigame.transform.SetParent(Camera.main.transform, false);
                    Local.minigame.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Local.minigame.Begin(null);
                }
                Local.charges--;

                if (noMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
                CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt(); // Stop current movement
            },
            () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.SecurityGuard) && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && Local.remainingScrews < Mathf.Min(SecurityGuard.ventPrice, SecurityGuard.camPrice) && SubmergedCompatibility.IsSubmerged; },
            () =>
            {
                securityGuardChargesText?.text = securityGuardChargesText.text = string.Format(Tr.Get("hackerChargesText"), Local.charges, camMaxCharges);
                securityGuardCamButton.ActionButton.graphic.sprite = Helpers.IsMiraHQ ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image;
                securityGuardCamButton.ActionButton.OverrideText(Helpers.IsMiraHQ ?
                    TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) :
                    TranslationController.Instance.GetString(StringNames.SecurityCamsSystem));
                return CachedPlayer.LocalPlayer.PlayerControl.CanMove && Local.charges > 0;
            },
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                securityGuardCamButton.IsEffectActive = false;
                securityGuardCamButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.CamsButton].Image,
            new Vector3(-1.8f, -0.06f, 0),
            hm,
            hm.UseButton,
            KeyCode.Q,
            true,
            0f,
            () =>
            {
                securityGuardCamButton.Timer = securityGuardCamButton.MaxTimer;
                if (Minigame.Instance)
                {
                    Local.minigame.ForceClose();
                }
                CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
            },
            false,
            Helpers.IsMiraHQ ? TranslationController.Instance.GetString(StringNames.SecurityLogsSystem) : TranslationController.Instance.GetString(StringNames.SecurityCamsSystem)
        );

        // Security Guard cam button charges
        securityGuardChargesText = GameObject.Instantiate(securityGuardCamButton.ActionButton.cooldownTimerText, securityGuardCamButton.ActionButton.cooldownTimerText.transform.parent);
        securityGuardChargesText.text = "";
        securityGuardChargesText.enableWordWrapping = false;
        securityGuardChargesText.transform.localScale = Vector3.one * 0.5f;
        securityGuardChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

    }
    public override void SetButtonCooldowns()
    {
        securityGuardButton.MaxTimer = SecurityGuard.cooldown;
        securityGuardCamButton.MaxTimer = SecurityGuard.cooldown;
        securityGuardCamButton.EffectDuration = SecurityGuard.camDuration;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}