using Object = UnityEngine.Object;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Hacker : RoleBase<Hacker>
{
    public static Color NameColor = new Color32(117, 250, 76, byte.MaxValue);

    private static CustomButton _hackerButton;
    public static CustomButton HackerVitalsButton;
    public static CustomButton HackerAdminTableButton;
    public static TMP_Text HackerAdminTableChargesText;
    public static TMP_Text HackerVitalsChargesText;

    public static float HackerTimer;
    private static SystemConsole _vitalsConsole;
    private static SystemConsole _doorLogConsole;
    public int ChargesAdminTable = 1;
    public int ChargesVitals = 1;
    public Minigame DoorLog;
    public int RechargedTasks = 2;

    public Minigame Vitals;

    public Hacker()
    {
        // write value init here
        StaticRoleType = CurrentRoleType = RoleType.Hacker;
    }

    public override Color RoleColor
    {
        get => NameColor;
    }

    // write configs here
    public static float Cooldown
    {
        get => CustomOptionHolder.HackerCooldown.GetFloat();
    }

    public static float Duration
    {
        get => CustomOptionHolder.HackerHackingDuration.GetFloat();
    }

    public static bool OnlyColorType
    {
        get => CustomOptionHolder.HackerOnlyColorType.GetBool();
    }

    public static float ToolsNumber
    {
        get => CustomOptionHolder.HackerToolsNumber.GetFloat();
    }

    public static int RechargeTasksNumber
    {
        get => Mathf.RoundToInt(CustomOptionHolder.HackerRechargeTasksNumber.GetFloat());
    }

    public static bool NoMove
    {
        get => CustomOptionHolder.HackerNoMove.GetBool();
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

    public static void MakeButtons(HudManager hm)
    {
        _hackerButton = new(() => { HackerTimer = Duration; }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && PlayerControl.LocalPlayer.IsAlive(); }, () => { return true; }, () =>
        {
            _hackerButton.Timer = _hackerButton.MaxTimer;
            _hackerButton.IsEffectActive = false;
            _hackerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, AssetLoader.HackerButton, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilityPrimary, true, 0f, () => { _hackerButton.Timer = _hackerButton.MaxTimer; }, false, Tr.Get(TrKey.HackerText));

        HackerAdminTableButton = new(() =>
        {
            PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement
            Local.ChargesAdminTable--;
            HudManager.Instance.ToggleMapVisible(new()
            {
                Mode = MapOptions.Modes.CountOverlay,
                AllowMovementWhileMapOpen = !NoMove,
                ShowLivePlayerPosition = true,
                IncludeDeadBodies = true,
            });
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && MapSettings.CouldUseAdmin && PlayerControl.LocalPlayer.IsAlive(); }, () =>
        {
            if (HackerAdminTableChargesText != null || HackerVitalsChargesText != null)
            {
                var format = Tr.Get(TrKey.HackerChargesText);
                var text = string.Format(format, Local.ChargesAdminTable, ToolsNumber);
                HackerAdminTableChargesText?.text = text;
                HackerVitalsChargesText?.text = text;
            }

            return Local.ChargesAdminTable > 0 && MapSettings.CanUseAdmin;
            ;
        }, () =>
        {
            HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
            HackerAdminTableButton.IsEffectActive = false;
            HackerAdminTableButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, GetAdminSprite(), ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CrewmateAbilitySecondary, true, 0f, () =>
        {
            HackerAdminTableButton.Timer = HackerAdminTableButton.MaxTimer;
            if (!HackerVitalsButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
            if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
        }, Helpers.GetOption(ByteOptionNames.MapId) == 3, FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin));

        // Hacker Admin Table Charges
        HackerAdminTableChargesText = Object.Instantiate(HackerAdminTableButton.ActionButton.cooldownTimerText, HackerAdminTableButton.ActionButton.cooldownTimerText.transform.parent);
        HackerAdminTableChargesText.text = "";
        HackerAdminTableChargesText.enableWordWrapping = false;
        HackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
        HackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        HackerVitalsButton = new(() =>
        {
            if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
            {
                if (Local.Vitals == null)
                {
                    if (_vitalsConsole == null)
                    {
                        var consoles = Object.FindObjectsOfType<SystemConsole>();
                        for (var i = 0; i < consoles.Length; i++)
                        {
                            if (consoles[i].gameObject.name.Contains("panel_vitals"))
                            {
                                _vitalsConsole = consoles[i];
                                break;
                            }
                        }
                    }

                    if (_vitalsConsole == null || Camera.main == null) return;
                    Local.Vitals = Object.Instantiate(_vitalsConsole.MinigamePrefab, Camera.main.transform, false);
                }

                Local.Vitals.transform.SetParent(Camera.main.transform, false);
                Local.Vitals.transform.localPosition = new(0.0f, 0.0f, -50f);
                Local.Vitals.Begin(null);
            }
            else
            {
                if (Local.DoorLog == null)
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
                    Local.DoorLog = Object.Instantiate(_doorLogConsole.MinigamePrefab, Camera.main.transform, false);
                }

                Local.DoorLog.transform.SetParent(Camera.main.transform, false);
                Local.DoorLog.transform.localPosition = new(0.0f, 0.0f, -50f);
                Local.DoorLog.Begin(null);
            }

            if (NoMove) PlayerControl.LocalPlayer.moveable = false;
            PlayerControl.LocalPlayer.NetTransform.Halt(); // Stop current movement

            Local.ChargesVitals--;
        }, () => { return PlayerControl.LocalPlayer.IsRole(RoleType.Hacker) && MapSettings.CouldUseVitals && PlayerControl.LocalPlayer.IsAlive() && Helpers.GetOption(ByteOptionNames.MapId) != 0 && Helpers.GetOption(ByteOptionNames.MapId) != 3; }, () =>
        {
            HackerVitalsChargesText?.text = string.Format(Tr.Get(TrKey.HackerChargesText), Local.ChargesVitals, ToolsNumber);
            HackerVitalsButton.ActionButton.graphic.sprite = Helpers.IsMiraHq ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
            HackerVitalsButton.ActionButton.OverrideText(Helpers.IsMiraHq ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel));
            return Local.ChargesVitals > 0 && MapSettings.CanUseVitals;
        }, () =>
        {
            HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
            HackerVitalsButton.IsEffectActive = false;
            HackerVitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
        }, FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image, ButtonPosition.Layout, hm, hm.UseButton, AbilitySlot.CommonAbilitySecondary, true, 0f, () =>
        {
            HackerVitalsButton.Timer = HackerVitalsButton.MaxTimer;
            if (!HackerAdminTableButton.IsEffectActive) PlayerControl.LocalPlayer.moveable = true;
            if (Minigame.Instance)
            {
                if (Helpers.IsMiraHq) Local.DoorLog.ForceClose();
                else Local.Vitals.ForceClose();
            }
        }, false, Helpers.IsMiraHq ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel));

        // Hacker Vitals Charges
        HackerVitalsChargesText = Object.Instantiate(HackerVitalsButton.ActionButton.cooldownTimerText, HackerVitalsButton.ActionButton.cooldownTimerText.transform.parent);
        HackerVitalsChargesText.text = "";
        HackerVitalsChargesText.enableWordWrapping = false;
        HackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
        HackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);
    }

    public static void SetButtonCooldowns()
    {
        _hackerButton.MaxTimer = Cooldown;
        HackerVitalsButton.MaxTimer = Cooldown;
        HackerAdminTableButton.MaxTimer = Cooldown;
        _hackerButton.EffectDuration = Duration;
        HackerVitalsButton.EffectDuration = Duration;
        HackerAdminTableButton.EffectDuration = Duration;
    }

    public static Sprite GetAdminSprite()
    {
        var mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
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
