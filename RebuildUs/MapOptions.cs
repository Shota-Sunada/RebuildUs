using Object = UnityEngine.Object;

namespace RebuildUs;

internal static class MapSettings
{
    // Set values
    internal static int MaxNumberOfMeetings = 10;
    internal static bool BlockSkippingInEmergencyMeetings;
    internal static bool NoVoteIsSelfVote;
    internal static bool HidePlayerNames;
    internal static bool AllowParallelMedBayScans;
    internal static bool HideOutOfSightNametags;

    internal static int RestrictDevices;
    internal static bool RestrictAdmin = true;
    internal static float RestrictAdminTime = 600f;
    private static float _restrictAdminTimeMax = 600f;
    private static bool _restrictAdminText = true;
    internal static bool RestrictCameras = true;
    internal static float RestrictCamerasTime = 600f;
    private static float _restrictCamerasTimeMax = 600f;
    private static bool _restrictCamerasText = true;
    internal static bool RestrictVitals = true;
    internal static float RestrictVitalsTime = 600f;
    private static float _restrictVitalsTimeMax = 600f;
    private static bool _restrictVitalsText = true;

    internal static bool GhostsSeeRoles = true;
    internal static bool GhostsSeeModifier = true;
    internal static bool GhostsSeeInformation = true;
    internal static bool GhostsSeeVotes = true;
    internal static bool ShowRoleSummary = true;
    internal static bool ShowLighterDarker;
    internal static bool BetterSabotageMap = false;
    internal static bool ForceNormalSabotageMap = false;
    internal static bool TransparentMap = false;
    internal static bool HideFakeTasks = false;

    internal static bool ShowVentsOnMap = true;
    internal static bool ShowChatNotifications = true;
    internal static CustomGamemode GameMode = CustomGamemode.Classic;

    internal static bool EnableDiscordAutoMute = true;
    internal static bool EnableDiscordEmbed = true;

    // Updating values
    internal static int MeetingsCount;
    internal static List<SurvCamera> CamerasToAdd = [];
    internal static List<Vent> VentsToSeal = [];
    internal static Dictionary<byte, PoolablePlayer> PlayerIcons = [];

    private static TextMeshPro _adminTimerText;
    private static TextMeshPro _camerasTimerText;
    private static TextMeshPro _vitalsTimerText;

    internal static bool CanUseAdmin
    {
        get => RestrictDevices == 0 || RestrictAdminTime > 0f;
    }

    internal static bool CouldUseAdmin
    {
        get => RestrictDevices == 0 || !RestrictAdmin || _restrictAdminTimeMax > 0f;
    }

    internal static bool CanUseCameras
    {
        get => RestrictDevices == 0 || !RestrictCameras || RestrictCamerasTime > 0f;
    }

    internal static bool CouldUseCameras
    {
        get => RestrictDevices == 0 || !RestrictCameras || _restrictCamerasTimeMax > 0f;
    }

    internal static bool CanUseVitals
    {
        get => RestrictDevices == 0 || !RestrictVitals || RestrictVitalsTime > 0f;
    }

    internal static bool CouldUseVitals
    {
        get => RestrictDevices == 0 || !RestrictVitals || _restrictVitalsTimeMax > 0f;
    }

    internal static void ClearAndReloadMapOptions()
    {
        MeetingsCount = 0;
        CamerasToAdd = [];
        VentsToSeal = [];
        PlayerIcons = [];

        MaxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.MaxNumberOfMeetings.GetSelection());
        BlockSkippingInEmergencyMeetings = CustomOptionHolder.BlockSkippingInEmergencyMeetings.GetBool();
        NoVoteIsSelfVote = CustomOptionHolder.NoVoteIsSelfVote.GetBool();
        HidePlayerNames = CustomOptionHolder.HidePlayerNames.GetBool();
        AllowParallelMedBayScans = CustomOptionHolder.AllowParallelMedBayScans.GetBool();
        HideOutOfSightNametags = CustomOptionHolder.HideOutOfSightNametags.GetBool();

        RestrictDevices = CustomOptionHolder.RestrictDevices.GetSelection();
        RestrictAdmin = CustomOptionHolder.RestrictAdmin.GetBool();
        RestrictAdminTime = _restrictAdminTimeMax = CustomOptionHolder.RestrictAdminTime.GetFloat();
        _restrictAdminText = CustomOptionHolder.RestrictAdminText.GetBool();
        RestrictCameras = CustomOptionHolder.RestrictCameras.GetBool();
        RestrictCamerasTime = _restrictCamerasTimeMax = CustomOptionHolder.RestrictCamerasTime.GetFloat();
        _restrictCamerasText = CustomOptionHolder.RestrictCamerasText.GetBool();
        _restrictVitalsText = CustomOptionHolder.RestrictVitalsText.GetBool();
        RestrictVitals = CustomOptionHolder.RestrictVitals.GetBool();
        RestrictVitalsTime = _restrictVitalsTimeMax = CustomOptionHolder.RestrictVitalsTime.GetFloat();

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
    }

    internal static void ResetDeviceTimes()
    {
        RestrictAdminTime = _restrictAdminTimeMax;
        RestrictCamerasTime = _restrictCamerasTimeMax;
        RestrictVitalsTime = _restrictVitalsTimeMax;
    }

    internal static void MeetingEndedUpdate()
    {
        ClearTimerText();
        UpdateTimerText();
    }

    private static void UpdateTimerText()
    {
        if (RestrictDevices == 0 || (!_restrictAdminText && !_restrictCamerasText && !_restrictVitalsText)) return;

        if (FastDestroyableSingleton<HudManager>.Instance == null) return;

        // Admin
        if (_restrictAdminText)
        {
            _adminTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            float y = -4.0f;
            if (_restrictCamerasText) y += 0.2f;

            if (_restrictVitalsText) y += 0.2f;

            _adminTimerText.transform.localPosition = new(-3.5f, y, 0);
            _adminTimerText.text = RestrictAdminTime > 0 ? string.Format(Tr.Get(TrKey.AdminText), RestrictAdminTime.ToString("0.00")) : Tr.Get(TrKey.AdminRanOut);
            _adminTimerText.gameObject.SetActive(true);
        }

        // Cameras
        if (_restrictCamerasText)
        {
            _camerasTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            float y = -4.0f;
            if (_restrictVitalsText) y += 0.2f;

            _camerasTimerText.transform.localPosition = new(-3.5f, y, 0);
            _camerasTimerText.text = RestrictCamerasTime > 0 ? string.Format(Tr.Get(TrKey.CamerasText), RestrictCamerasTime.ToString("0.00")) : Tr.Get(TrKey.CamerasRanOut);
            _camerasTimerText.gameObject.SetActive(true);
        }

        // Vitals
        if (_restrictVitalsText)
        {
            _vitalsTimerText = Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskPanel.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
            _vitalsTimerText.transform.localPosition = new(-3.5f, -4.0f, 0);
            _vitalsTimerText.text = RestrictVitalsTime > 0 ? string.Format(Tr.Get(TrKey.VitalsText), RestrictVitalsTime.ToString("0.00")) : Tr.Get(TrKey.VitalsRanOut);
            _vitalsTimerText.gameObject.SetActive(true);
        }
    }

    private static void ClearTimerText()
    {
        if (_adminTimerText != null) Object.Destroy(_adminTimerText);

        _adminTimerText = null;
        if (_camerasTimerText != null) Object.Destroy(_camerasTimerText);

        _camerasTimerText = null;
        if (_vitalsTimerText != null) Object.Destroy(_vitalsTimerText);

        _vitalsTimerText = null;
    }
}