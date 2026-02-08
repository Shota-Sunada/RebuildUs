using Object = UnityEngine.Object;

namespace RebuildUs;

internal static class MapSettings
{
    // Set values
    internal static int MaxNumberOfMeetings = 10;
    internal static bool BlockSkippingInEmergencyMeetings;
    public static bool NoVoteIsSelfVote;
    internal static bool HidePlayerNames;
    internal static bool AllowParallelMedBayScans;
    internal static bool HideOutOfSightNametags;

    internal static int RestrictDevices;
    internal static bool RestrictAdmin = true;
    internal static float RestrictAdminTime = 600f;
    internal static float RestrictAdminTimeMax = 600f;
    internal static bool RestrictAdminText = true;
    internal static bool RestrictCameras = true;
    internal static float RestrictCamerasTime = 600f;
    internal static float RestrictCamerasTimeMax = 600f;
    internal static bool RestrictCamerasText = true;
    internal static bool RestrictVitals = true;
    internal static float RestrictVitalsTime = 600f;
    internal static float RestrictVitalsTimeMax = 600f;
    internal static bool RestrictVitalsText = true;

    internal static bool GhostsSeeRoles = true;
    internal static bool GhostsSeeModifier = true;
    internal static bool GhostsSeeInformation = true;
    internal static bool GhostsSeeVotes = true;
    internal static bool ShowRoleSummary = true;
    internal static bool ShowLighterDarker;
    public static bool BetterSabotageMap = false;
    public static bool ForceNormalSabotageMap = false;
    public static bool TransparentMap = false;
    public static bool HideFakeTasks = false;

    public static bool ShowVentsOnMap = true;
    public static bool ShowChatNotifications = true;

    public static CustomGameMode GameMode = CustomGameMode.Roles;
    public static float GamemodeMatchDuration = CustomOptionHolder.GamemodeMatchDuration.GetFloat();
    public static float GamemodeKillCooldown = CustomOptionHolder.GamemodeKillCooldown.GetFloat();
    public static bool GamemodeEnableFlashlight = CustomOptionHolder.GamemodeEnableFlashlight.GetBool();
    public static float GamemodeFlashlightRange = CustomOptionHolder.GamemodeFlashlightRange.GetFloat();
    public static float GamemodeReviveTime = CustomOptionHolder.GamemodeReviveTime.GetFloat();
    public static float GamemodeInvincibilityTime = CustomOptionHolder.GamemodeInvincibilityTimeAfterRevive.GetFloat();

    public static bool EnableDiscordAutoMute = true;
    public static bool EnableDiscordEmbed = true;

    // Updating values
    public static int MeetingsCount;
    public static List<SurvCamera> CamerasToAdd = [];
    public static List<Vent> VentsToSeal = [];
    public static Dictionary<byte, PoolablePlayer> PlayerIcons = [];

    public static TextMeshPro AdminTimerText;
    public static TextMeshPro CamerasTimerText;
    public static TextMeshPro VitalsTimerText;

    public static bool CanUseAdmin
    {
        get => RestrictDevices == 0 || RestrictAdminTime > 0f;
    }

    public static bool CouldUseAdmin
    {
        get => RestrictDevices == 0 || !RestrictAdmin || RestrictAdminTimeMax > 0f;
    }

    public static bool CanUseCameras
    {
        get => RestrictDevices == 0 || !RestrictCameras || RestrictCamerasTime > 0f;
    }

    public static bool CouldUseCameras
    {
        get => RestrictDevices == 0 || !RestrictCameras || RestrictCamerasTimeMax > 0f;
    }

    public static bool CanUseVitals
    {
        get => RestrictDevices == 0 || !RestrictVitals || RestrictVitalsTime > 0f;
    }

    public static bool CouldUseVitals
    {
        get => RestrictDevices == 0 || !RestrictVitals || RestrictVitalsTimeMax > 0f;
    }

    public static void ClearAndReloadMapOptions()
    {
        MeetingsCount = 0;
        CamerasToAdd = [];
        VentsToSeal = [];
        PlayerIcons = [];
        ;

        MaxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.MaxNumberOfMeetings.GetSelection());
        BlockSkippingInEmergencyMeetings = CustomOptionHolder.BlockSkippingInEmergencyMeetings.GetBool();
        NoVoteIsSelfVote = CustomOptionHolder.NoVoteIsSelfVote.GetBool();
        HidePlayerNames = CustomOptionHolder.HidePlayerNames.GetBool();
        AllowParallelMedBayScans = CustomOptionHolder.AllowParallelMedBayScans.GetBool();
        HideOutOfSightNametags = CustomOptionHolder.HideOutOfSightNametags.GetBool();

        RestrictDevices = CustomOptionHolder.RestrictDevices.GetSelection();
        RestrictAdmin = CustomOptionHolder.RestrictAdmin.GetBool();
        RestrictAdminTime = RestrictAdminTimeMax = CustomOptionHolder.RestrictAdminTime.GetFloat();
        RestrictAdminText = CustomOptionHolder.RestrictAdminText.GetBool();
        RestrictCameras = CustomOptionHolder.RestrictCameras.GetBool();
        RestrictCamerasTime = RestrictCamerasTimeMax = CustomOptionHolder.RestrictCamerasTime.GetFloat();
        RestrictCamerasText = CustomOptionHolder.RestrictCamerasText.GetBool();
        RestrictVitalsText = CustomOptionHolder.RestrictVitalsText.GetBool();
        RestrictVitals = CustomOptionHolder.RestrictVitals.GetBool();
        RestrictVitalsTime = RestrictVitalsTimeMax = CustomOptionHolder.RestrictVitalsTime.GetFloat();

        ClearTimerText();
        UpdateTimerText();

        GhostsSeeRoles = RebuildUs.GhostsSeeRoles.Value;
        GhostsSeeModifier = RebuildUs.GhostsSeeModifier.Value;
        GhostsSeeInformation = RebuildUs.GhostsSeeInformation.Value;
        GhostsSeeVotes = RebuildUs.GhostsSeeVotes.Value;
        ShowRoleSummary = RebuildUs.ShowRoleSummary.Value;
        ShowLighterDarker = RebuildUs.ShowLighterDarker.Value;
        ShowVentsOnMap = RebuildUs.ShowVentsOnMap.Value;
        ShowChatNotifications = RebuildUs.ShowChatNotifications.Value;

        EnableDiscordAutoMute = CustomOptionHolder.EnableDiscordAutoMute.GetBool();
        EnableDiscordEmbed = CustomOptionHolder.EnableDiscordEmbed.GetBool();

        DiscordModManager.Initialize();
    }

    public static void ResetDeviceTimes()
    {
        RestrictAdminTime = RestrictAdminTimeMax;
        RestrictCamerasTime = RestrictCamerasTimeMax;
        RestrictVitalsTime = RestrictVitalsTimeMax;
    }

    public static void MeetingEndedUpdate()
    {
        ClearTimerText();
        UpdateTimerText();
    }

    public static void UpdateTimerText()
    {
        if (RestrictDevices == 0 || (!RestrictAdminText && !RestrictCamerasText && !RestrictVitalsText)) return;
        if (FastDestroyableSingleton<HudManager>.Instance == null) return;

        // Admin
        if (RestrictAdminText)
        {
            AdminTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            var y = -4.0f;
            if (RestrictCamerasText) y += 0.2f;
            if (RestrictVitalsText) y += 0.2f;
            AdminTimerText.transform.localPosition = new(-3.5f, y, 0);
            AdminTimerText.text = RestrictAdminTime > 0 ? string.Format(Tr.Get(TrKey.AdminText), RestrictAdminTime.ToString("0.00")) : Tr.Get(TrKey.AdminRanOut);
            AdminTimerText.gameObject.SetActive(true);
        }

        // Cameras
        if (RestrictCamerasText)
        {
            CamerasTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            var y = -4.0f;
            if (RestrictVitalsText) y += 0.2f;
            CamerasTimerText.transform.localPosition = new(-3.5f, y, 0);
            CamerasTimerText.text = RestrictCamerasTime > 0 ? string.Format(Tr.Get(TrKey.CamerasText), RestrictCamerasTime.ToString("0.00")) : Tr.Get(TrKey.CamerasRanOut);
            CamerasTimerText.gameObject.SetActive(true);
        }

        // Vitals
        if (RestrictVitalsText)
        {
            VitalsTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            VitalsTimerText.transform.localPosition = new(-3.5f, -4.0f, 0);
            VitalsTimerText.text = RestrictVitalsTime > 0 ? string.Format(Tr.Get(TrKey.VitalsText), RestrictVitalsTime.ToString("0.00")) : Tr.Get(TrKey.VitalsRanOut);
            VitalsTimerText.gameObject.SetActive(true);
        }
    }

    private static void ClearTimerText()
    {
        if (AdminTimerText != null) Object.Destroy(AdminTimerText);
        AdminTimerText = null;
        if (CamerasTimerText != null) Object.Destroy(CamerasTimerText);
        CamerasTimerText = null;
        if (VitalsTimerText != null) Object.Destroy(VitalsTimerText);
        VitalsTimerText = null;
    }
}
