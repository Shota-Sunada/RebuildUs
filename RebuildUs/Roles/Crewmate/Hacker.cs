using Epic.OnlineServices;

namespace RebuildUs.Roles.Crewmate;

[HarmonyPatch]
public class Hacker : RoleBase<Hacker>
{
    public static Color RoleColor = new Color32(117, 250, 76, byte.MaxValue);
    private static CustomButton hackerButton;
    public static CustomButton hackerVitalsButton;
    public static CustomButton hackerAdminTableButton;
    public static TMP_Text hackerAdminTableChargesText;
    public static TMP_Text hackerVitalsChargesText;

    public Minigame vitals = null;
    public Minigame doorLog = null;

    // write configs here
    public static float cooldown { get { return CustomOptionHolder.hackerCooldown.GetFloat(); } }
    public static float duration { get { return CustomOptionHolder.hackerHackingDuration.GetFloat(); } }
    public static bool onlyColorType { get { return CustomOptionHolder.hackerOnlyColorType.GetBool(); } }
    public static float toolsNumber { get { return CustomOptionHolder.hackerToolsNumber.GetFloat(); } }
    public static int rechargeTasksNumber { get { return Mathf.RoundToInt(CustomOptionHolder.hackerRechargeTasksNumber.GetFloat()); } }
    public static bool noMove { get { return CustomOptionHolder.hackerNoMove.GetBool(); } }

    public float hackerTimer = 0f;
    public int rechargedTasks = 2;
    public int chargesVitals = 1;
    public int chargesAdminTable = 1;

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
        hackerTimer -= Time.deltaTime;
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive())
        {
            var (playerCompleted, _) = TasksHandler.TaskInfo(Player.Data);
            if (playerCompleted == rechargedTasks)
            {
                rechargedTasks += rechargeTasksNumber;
                if (toolsNumber > chargesVitals) chargesVitals++;
                if (toolsNumber > chargesAdminTable) chargesAdminTable++;
            }
        }
    }
    public override void OnKill(PlayerControl target) { }
    public override void OnDeath(PlayerControl killer = null) { }
    public override void OnFinishShipStatusBegin() { }
    public override void HandleDisconnect(PlayerControl player, DisconnectReasons reason) { }
    public override void MakeButtons(HudManager hm)
    {
        hackerButton = new CustomButton(
                () =>
                {
                    hackerTimer = duration;
                },
                () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker) && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
                () => { return true; },
                () =>
                {
                    hackerButton.Timer = hackerButton.MaxTimer;
                    hackerButton.IsEffectActive = false;
                    hackerButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
                },
                AssetLoader.HackerButton,
                new Vector3(0f, 1f, 0),
                hm,
                hm.UseButton,
                KeyCode.F,
                true,
                0f,
                () => { hackerButton.Timer = hackerButton.MaxTimer; }
            )
        {
            ButtonText = Tr.Get("HackerText")
        };

        hackerAdminTableButton = new CustomButton(
           () =>
           {
               if (!MapBehaviour.Instance.isActiveAndEnabled)
               {
                   FastDestroyableSingleton<MapBehaviour>.Instance.ShowCountOverlay(noMove, true, true);
               }
               CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt(); // Stop current movement
               Local.chargesAdminTable--;
           },
           () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker) && ModMapOptions.couldUseAdmin && CachedPlayer.LocalPlayer.PlayerControl.IsAlive(); },
           () =>
           {
               hackerAdminTableChargesText?.text = hackerVitalsChargesText.text = string.Format(Tr.Get("hackerChargesText"), Local.chargesAdminTable, toolsNumber);
               return Local.chargesAdminTable > 0 && ModMapOptions.canUseAdmin; ;
           },
           () =>
           {
               hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
               hackerAdminTableButton.IsEffectActive = false;
               hackerAdminTableButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           Hacker.getAdminSprite(),
           new Vector3(-1.8f, -0.06f, 0),
           hm,
           hm.UseButton,
           KeyCode.Q,
           true,
           0f,
           () =>
           {
               hackerAdminTableButton.Timer = hackerAdminTableButton.MaxTimer;
               if (!hackerVitalsButton.IsEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
               if (MapBehaviour.Instance && MapBehaviour.Instance.isActiveAndEnabled) MapBehaviour.Instance.Close();
           },
           Helpers.GetOption(ByteOptionNames.MapId) == 3,
           FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Admin)
        );

        // Hacker Admin Table Charges
        hackerAdminTableChargesText = UnityEngine.Object.Instantiate(hackerAdminTableButton.ActionButton.cooldownTimerText, hackerAdminTableButton.ActionButton.cooldownTimerText.transform.parent);
        hackerAdminTableChargesText.text = "";
        hackerAdminTableChargesText.enableWordWrapping = false;
        hackerAdminTableChargesText.transform.localScale = Vector3.one * 0.5f;
        hackerAdminTableChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

        hackerVitalsButton = new CustomButton(
           () =>
           {
               if (Helpers.GetOption(ByteOptionNames.MapId) != 1)
               {
                   if (Local.vitals == null)
                   {
                       var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("panel_vitals"));
                       if (e == null || Camera.main == null) return;
                       Local.vitals = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                   }
                   Local.vitals.transform.SetParent(Camera.main.transform, false);
                   Local.vitals.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                   Local.vitals.Begin(null);
               }
               else
               {
                   if (Local.doorLog == null)
                   {
                       var e = UnityEngine.Object.FindObjectsOfType<SystemConsole>().FirstOrDefault(x => x.gameObject.name.Contains("SurvLogConsole"));
                       if (e == null || Camera.main == null) return;
                       Local.doorLog = UnityEngine.Object.Instantiate(e.MinigamePrefab, Camera.main.transform, false);
                   }
                   Local.doorLog.transform.SetParent(Camera.main.transform, false);
                   Local.doorLog.transform.localPosition = new Vector3(0.0f, 0.0f, -50f);
                   Local.doorLog.Begin(null);
               }

               if (noMove) CachedPlayer.LocalPlayer.PlayerControl.moveable = false;
               CachedPlayer.LocalPlayer.PlayerControl.NetTransform.Halt(); // Stop current movement

               Local.chargesVitals--;
           },
           () => { return CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker) && ModMapOptions.couldUseVitals && CachedPlayer.LocalPlayer.PlayerControl.IsAlive() && Helpers.GetOption(ByteOptionNames.MapId) != 0 && Helpers.GetOption(ByteOptionNames.MapId) != 3; },
           () =>
           {
               hackerVitalsChargesText?.text = string.Format(Tr.Get("hackerChargesText"), Local.chargesVitals, toolsNumber);
               hackerVitalsButton.ActionButton.graphic.sprite = Helpers.GetOption(ByteOptionNames.MapId) == 1 ? FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.DoorLogsButton].Image : FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image;
               hackerVitalsButton.ActionButton.OverrideText(Helpers.GetOption(ByteOptionNames.MapId) == 1 ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel));
               return Local.chargesVitals > 0 && ModMapOptions.canUseVitals;
           },
           () =>
           {
               hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
               hackerVitalsButton.IsEffectActive = false;
               hackerVitalsButton.ActionButton.cooldownTimerText.color = Palette.EnabledColor;
           },
           FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.VitalsButton].Image,
           new Vector3(-2.7f, -0.06f, 0),
           hm,
           hm.UseButton,
           KeyCode.Q,
           true,
           0f,
           () =>
           {
               hackerVitalsButton.Timer = hackerVitalsButton.MaxTimer;
               if (!hackerAdminTableButton.IsEffectActive) CachedPlayer.LocalPlayer.PlayerControl.moveable = true;
               if (Minigame.Instance)
               {
                   if (Helpers.GetOption(ByteOptionNames.MapId) == 1) Local.doorLog.ForceClose();
                   else Local.vitals.ForceClose();
               }
           },
           false,
           Helpers.GetOption(ByteOptionNames.MapId) == 1 ? TranslationController.Instance.GetString(StringNames.DoorlogLabel) : TranslationController.Instance.GetString(StringNames.VitalsLabel)
        );

        // Hacker Vitals Charges
        hackerVitalsChargesText = GameObject.Instantiate(hackerVitalsButton.ActionButton.cooldownTimerText, hackerVitalsButton.ActionButton.cooldownTimerText.transform.parent);
        hackerVitalsChargesText.text = "";
        hackerVitalsChargesText.enableWordWrapping = false;
        hackerVitalsChargesText.transform.localScale = Vector3.one * 0.5f;
        hackerVitalsChargesText.transform.localPosition += new Vector3(-0.05f, 0.7f, 0);

    }
    public override void SetButtonCooldowns()
    {
        hackerButton.MaxTimer = Hacker.cooldown;
        hackerVitalsButton.MaxTimer = Hacker.cooldown;
        hackerAdminTableButton.MaxTimer = Hacker.cooldown;
        hackerButton.EffectDuration = Hacker.duration;
        hackerVitalsButton.EffectDuration = Hacker.duration;
        hackerAdminTableButton.EffectDuration = Hacker.duration;
    }

    public static Sprite getAdminSprite()
    {
        byte mapId = GameOptionsManager.Instance.CurrentGameOptions.MapId;
        var button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.PolusAdminButton]; // Polus
        if (mapId is 0 or 3) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AdminMapButton]; // Skeld || Dleks
        else if (mapId == 1) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.MIRAAdminButton]; // Mira HQ
        else if (mapId == 4) button = FastDestroyableSingleton<HudManager>.Instance.UseButton.fastUseSettings[ImageNames.AirshipAdminButton]; // Airship

        return button.Image;
    }

    // write functions here

    public override void Clear()
    {
        // reset configs here
        Players.Clear();
    }
}