namespace RebuildUs
{
    static class ModMapOptions
    {
        // Set values
        public static int MaxNumberOfMeetings = 10;
        public static bool BlockSkippingInEmergencyMeetings = false;
        public static bool NoVoteIsSelfVote = false;
        public static bool HidePlayerNames = false;
        public static bool AllowParallelMedBayScans = false;
        public static bool HideOutOfSightNametags = false;

        public static int RestrictDevices = 0;
        public static bool RestrictAdmin = true;
        public static float RestrictAdminTime = 600f;
        public static float RestrictAdminTimeMax = 600f;
        public static bool RestrictAdminText = true;
        public static bool RestrictCameras = true;
        public static float RestrictCamerasTime = 600f;
        public static float RestrictCamerasTimeMax = 600f;
        public static bool RestrictCamerasText = true;
        public static bool RestrictVitals = true;
        public static float RestrictVitalsTime = 600f;
        public static float RestrictVitalsTimeMax = 600f;
        public static bool RestrictVitalsText = true;

        public static bool GhostsSeeRoles = true;
        public static bool GhostsSeeModifier = true;
        public static bool GhostsSeeInformation = true;
        public static bool GhostsSeeVotes = true;
        public static bool ShowRoleSummary = true;
        public static bool ShowLighterDarker = false;
        public static bool BetterSabotageMap = false;
        public static bool ForceNormalSabotageMap = false;
        public static bool TransparentMap = false;
        public static bool HideFakeTasks = false;

        public static bool ShowVentsOnMap = true;
        public static bool ShowChatNotifications = true;
        public static CustomGamemodes GameMode = CustomGamemodes.Classic;

        // Updating values
        public static int MeetingsCount = 0;
        public static List<SurvCamera> CamerasToAdd = [];
        public static List<Vent> VentsToSeal = [];
        public static Dictionary<byte, PoolablePlayer> PlayerIcons = [];

        public static TextMeshPro AdminTimerText = null;
        public static TextMeshPro CamerasTimerText = null;
        public static TextMeshPro VitalsTimerText = null;

        public static void ClearAndReloadMapOptions()
        {
            MeetingsCount = 0;
            CamerasToAdd = [];
            VentsToSeal = [];
            PlayerIcons = []; ;

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
        }

        public static void ResetDeviceTimes()
        {
            RestrictAdminTime = RestrictAdminTimeMax;
            RestrictCamerasTime = RestrictCamerasTimeMax;
            RestrictVitalsTime = RestrictVitalsTimeMax;
        }

        public static bool CanUseAdmin
        {
            get
            {
                return RestrictDevices == 0 || RestrictAdminTime > 0f;
            }
        }

        public static bool CouldUseAdmin
        {
            get
            {
                return RestrictDevices == 0 || !RestrictAdmin || RestrictAdminTimeMax > 0f;
            }
        }

        public static bool CanUseCameras
        {
            get
            {
                return RestrictDevices == 0 || !RestrictCameras || RestrictCamerasTime > 0f;
            }
        }

        public static bool CouldUseCameras
        {
            get
            {
                return RestrictDevices == 0 || !RestrictCameras || RestrictCamerasTimeMax > 0f;
            }
        }

        public static bool CanUseVitals
        {
            get
            {
                return RestrictDevices == 0 || !RestrictVitals || RestrictVitalsTime > 0f;
            }
        }

        public static bool CouldUseVitals
        {
            get
            {
                return RestrictDevices == 0 || !RestrictVitals || RestrictVitalsTimeMax > 0f;
            }
        }
        public static void MeetingEndedUpdate()
        {
            ClearTimerText();
            UpdateTimerText();
        }

        public static void UpdateTimerText()
        {
            if (RestrictDevices == 0 || (!RestrictAdminText && !RestrictCamerasText && !RestrictVitalsText))
                return;
            if (FastDestroyableSingleton<HudManager>.Instance == null)
                return;

            // Admin
            if (RestrictAdminText)
            {
                AdminTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                float y = -4.0f;
                if (RestrictCamerasText)
                    y += 0.2f;
                if (RestrictVitalsText)
                    y += 0.2f;
                AdminTimerText.transform.localPosition = new Vector3(-3.5f, y, 0);
                if (RestrictAdminTime > 0)
                    AdminTimerText.text = String.Format(Tr.Get("Hud.AdminText"), RestrictAdminTime.ToString("0.00"));
                else
                    AdminTimerText.text = Tr.Get("Hud.AdminRanOut");
                AdminTimerText.gameObject.SetActive(true);
            }

            // Cameras
            if (RestrictCamerasText)
            {
                CamerasTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                float y = -4.0f;
                if (RestrictVitalsText)
                    y += 0.2f;
                CamerasTimerText.transform.localPosition = new Vector3(-3.5f, y, 0);
                if (RestrictCamerasTime > 0)
                    CamerasTimerText.text = String.Format(Tr.Get("Hud.CamerasText"), RestrictCamerasTime.ToString("0.00"));
                else
                    CamerasTimerText.text = Tr.Get("Hud.CamerasRanOut");
                CamerasTimerText.gameObject.SetActive(true);
            }

            // Vitals
            if (RestrictVitalsText)
            {
                VitalsTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<TaskPanelBehaviour>.Instance.taskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                VitalsTimerText.transform.localPosition = new Vector3(-3.5f, -4.0f, 0);
                if (RestrictVitalsTime > 0)
                    VitalsTimerText.text = String.Format(Tr.Get("Hud.VitalsText"), RestrictVitalsTime.ToString("0.00"));
                else
                    VitalsTimerText.text = Tr.Get("Hud.VitalsRanOut");
                VitalsTimerText.gameObject.SetActive(true);
            }
        }

        private static void ClearTimerText()
        {
            if (AdminTimerText != null)
                UnityEngine.Object.Destroy(AdminTimerText);
            AdminTimerText = null;
            if (CamerasTimerText != null)
                UnityEngine.Object.Destroy(CamerasTimerText);
            CamerasTimerText = null;
            if (VitalsTimerText != null)
                UnityEngine.Object.Destroy(VitalsTimerText);
            VitalsTimerText = null;

        }
    }
}