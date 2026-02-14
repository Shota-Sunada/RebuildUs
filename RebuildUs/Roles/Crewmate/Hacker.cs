namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Hacker : RoleBase<Hacker>
{
    public static Color NameColor = new Color32(117, 250, 76, byte.MaxValue);
    public override Color RoleColor => NameColor;
    private static CustomButton HackerButton;
    public static CustomButton HackerVitalsButton;
    public static CustomButton HackerAdminTableButton;
    public static TMP_Text HackerAdminTableChargesText;
    public static TMP_Text HackerVitalsChargesText;

    public Minigame Vitals = null;
    public Minigame DoorLog = null;

    // write configs here
    public static float Cooldown { get { return CustomOptionHolder.HackerCooldown.GetFloat(); } }
    public static float Duration { get { return CustomOptionHolder.HackerHackingDuration.GetFloat(); } }
    public static bool OnlyColorType { get { return CustomOptionHolder.HackerOnlyColorType.GetBool(); } }
    public static float ToolsNumber { get { return CustomOptionHolder.HackerToolsNumber.GetFloat(); } }
    public static int RechargeTasksNumber { get { return Mathf.RoundToInt(CustomOptionHolder.HackerRechargeTasksNumber.GetFloat()); } }
    public static bool NoMove { get { return CustomOptionHolder.HackerNoMove.GetBool(); } }

    public static float HackerTimer = 0f;
    public int RechargedTasks = 2;
    public int ChargesVitals = 1;
    public int ChargesAdminTable = 1;

    public Hacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Hacker;
    }

    public override void OnMeetingStart() { }
    public override void OnMeetingEnd() { }
    public override void OnIntroEnd() { }
    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && PlayerControl.LocalPlayer.IsAlive())
        {
            var (playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
            if (playerCompleted == RechargedTasks)
            {
                RechargedTasks += RechargeTasksNumber;
                if (ToolsNumber > ChargesVitals) ChargesVitals++;
                if (ToolsNumber > ChargesAdminTable) ChargesAdminTable++;
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    private static SystemConsole _vitalsConsole;
    private static SystemConsole _doorLogConsole;

    public static void MakeButtons(HudManager hm)
    {
        HackerButton = new CustomButton(
            () =>
            {
                HackerTimer = Duration;
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && PlayerControl.LocalPlayer.IsAlive(); },
            () => { return true; },
            () =>
            {
                HackerButton.Timer = HackerButton.MaxTimer;
                HackerButton.IsEffectActive = false;
                HackerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            AssetLoader.HackerButton,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilityPrimary,
            true,
            0f,
            () => { HackerButton.Timer = HackerButton.MaxTimer; },
            false,
            Tr.Get(TranslateKey.HackerText)
        );

        HackerAdminTableButton = new CustomButton(
            () =>
            {
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
                Local.ChargesAdminTable--;
                HudManager.Instance.ToggleMapVisible(new()
                {
                    Mode = MapOptions.Modes.CountOverlay,
                    AllowMovementWhileMapOpen = !NoMove,
                    ShowLivePlayerPosition = true,
                    IncludeDeadBodies = true
                });
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && MapSettings.CouldUseAdmin && PlayerControl.LocalPlayer.IsAlive(); },
            () =>
            {
                if (HackerAdminTableChargesText != null || HackerVitalsChargesText != null)
                {
                    string format = Tr.Get(TranslateKey.HackerChargesText);
                    string text = string.Format(format, Local.ChargesAdminTable, ToolsNumber);
                    HackerAdminTableChargesText?.text = text;
                    HackerVitalsChargesText?.text = text;
                }
                return Local.ChargesAdminTable > 0 && MapSettings.CanUseAdmin; ;
            },
            () =>
            {
                HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
                HackerAdminTableButton.IsEffectActive = false;
                HackerAdminTableButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            GetAdminSprite(),
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CrewmateAbilitySecondary,
            true,
            0f,
            () =>
            {
                HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
                if (!HackerVitalsButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
            },
            Helpers.GetOption(ByteOptionNames.MapId) == 3,
            FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
        );

        // Hacker Admin Table Charges
        HackerAdminTableChargesText = UnityEngine.Object.Instantiate(HackerAdminTableButton.ActionButton.cooldownTimerText, HackerAdminTableButton.ActionButton.cooldownTimerText.transform.parent);
        HackerAdminTableChargesText.text = "";
        HackerAdminTableChargesText.enableWordWrapping = false;
        HackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
        HackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        HackerVitalsButton = new CustomButton(
            () =>
            {
                if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
                {
                    if (Local.Vitals == null)
                    {
                        if (_vitalsConsole == null)
                        {
                            var consoles = UnityEngine.Object.FindObjectsOfType<SystemConsole>();
                            for (int i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("panel_vitals"))
                                {
                                    _vitalsConsole = consoles[i];
                                    break;
                                }
                            }
                        }
                        if (_vitalsConsole == null || Camera.main == null) return;
                        Local.Vitals = UnityEngine.Object.Instantiate(_vitalsConsole.MinigamePrefab, Camera.main.transform, false);
                    }
                    Local.Vitals.transform.SetParent(Camera.main.transform, false);
                    Local.Vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Local.Vitals.Begin(null);
                }
                else
                {
                    if (Local.DoorLog == null)
                    {
                        if (_doorLogConsole == null)
                        {
                            var consoles = UnityEngine.Object.FindObjectsOfType<SystemConsole>();
                            for (int i = 0; i < consoles.Length; i++)
                            {
                                if (consoles[i].gameObject.name.Contains("SurvLogConsole"))
                                {
                                    _doorLogConsole = consoles[i];
                                    break;
                                }
                            }
                        }
                        if (_doorLogConsole == null || Camera.main == null) return;
                        Local.DoorLog = UnityEngine.Object.Instantiate(_doorLogConsole.MinigamePrefab, Camera.main.transform, false);
                    }
                    Local.DoorLog.transform.SetParent(Camera.main.transform, false);
                    Local.DoorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                    Local.DoorLog.Begin(null);
                }

                if (NoMove) PlayerControl.LocalPlayer.moveable = false;
                PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement

                Local.ChargesVitals--;
            },
            () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && MapSettings.CouldUseVitals && PlayerControl.LocalPlayer.IsAlive() && Helpers.GetOption(ByteOptionNames.MapId) != 0 && Helpers.GetOption(ByteOptionNames.MapId) != 3; },
            () =>
            {
                HackerVitalsChargesText?.text = string.Format(Tr.Get(TranslateKey.HackerChargesText), Local.ChargesVitals, ToolsNumber);
                HackerVitalsButton.ActionButton.graphic.sprite = Helpers.IsMiraHQ ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
                HackerVitalsButton.ActionButton.OverrideText(Helpers.IsMiraHQ ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel));
                return Local.ChargesVitals > 0 && MapSettings.CanUseVitals;
            },
            () =>
            {
                HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
                HackerVitalsButton.IsEffectActive = false;
                HackerVitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
            },
            FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image,
            ButtonPosition.Layout,
            hm,
            hm.UseButton,
            AbilitySlot.CommonAbilitySecondary,
            true,
            0f,
            () =>
            {
                HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
                if (!HackerAdminTableButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
                if (Minigame.Instance)
                {
                    if (Helpers.IsMiraHQ) Local.DoorLog.ForceClose();
                    else Local.Vitals.ForceClose();
                }
            },
            false,
            Helpers.IsMiraHQ ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel)
        );

        // Hacker Vitals Charges
        HackerVitalsChargesText = UnityEngine.Object.Instantiate(HackerVitalsButton.ActionButton.cooldownTimerText, HackerVitalsButton.ActionButton.cooldownTimerText.transform.parent);
        HackerVitalsChargesText.text = "";
        HackerVitalsChargesText.enableWordWrapping = false;
        HackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
        HackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

    }
    public static void SetButtonCooldowns()
    {
        HackerButton.MaxTimer = Cooldown;
        HackerVitalsButton.MaxTimer = Cooldown;
        HackerAdminTableButton.MaxTimer = Cooldown;
        HackerButton.EffectDuration = Duration;
        HackerVitalsButton.EffectDuration = Duration;
        HackerAdminTableButton.EffectDuration = Duration;
    }

    public static Sprite GetAdminSprite()
    {
        byte mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
        var button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
        if (mapId is 0 or 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
        else if (mapId == 1) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
        else if (mapId == 4) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship

        return button.Image;
    }

    // write functions here

    public static void Clear()
    {
        // reset configs here
        HackerTimer = 0f;
        Players.Clear();
    }
}