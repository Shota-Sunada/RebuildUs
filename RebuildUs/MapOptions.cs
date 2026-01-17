using System.Collections.Generic;
using UnityEngine;

namespace RebuildUs
{
    static class MapOptions
    {
        // Set values
        public static int MaxNumberOfMeetings = 10;
        public static bool BlockSkippingInEmergencyMeetings = false;
        public static bool NoVoteIsSelfVote = false;
        public static bool HidePlayerNames = false;
        public static bool AllowParallelMedBayScans = false;
        public static bool HideOutOfSightNametags = false;

        public static int restrictDevices = 0;
        public static bool restrictAdmin = true;
        public static float restrictAdminTime = 600f;
        public static float restrictAdminTimeMax = 600f;
        public static bool restrictAdminText = true;
        public static bool restrictCameras = true;
        public static float restrictCamerasTime = 600f;
        public static float restrictCamerasTimeMax = 600f;
        public static bool restrictCamerasText = true;
        public static bool restrictVitals = true;
        public static float restrictVitalsTime = 600f;
        public static float restrictVitalsTimeMax = 600f;
        public static bool restrictVitalsText = true;

        public static bool GhostsSeeRoles = true;
        public static bool GhostsSeeModifier = true;
        public static bool GhostsSeeInformation = true;
        public static bool GhostsSeeVotes = true;
        public static bool ShowRoleSummary = true;

        public static bool ShowLighterDarker = true;
        public static bool EnableSoundEffects = true;
        public static bool EnableHorseMode = false;
        public static bool ShieldFirstKill = false;
        public static bool ShowVentsOnMap = true;
        public static bool ShowChatNotifications = true;
        public static CustomGamemodes GameMode = CustomGamemodes.Classic;

        // Updating values
        public static int MeetingsCount = 0;
        public static List<SurvCamera> CamerasToAdd = [];
        public static List<Vent> VentsToSeal = [];
        public static Dictionary<byte, PoolablePlayer> PlayerIcons = [];
        public static string FirstKillName;
        public static PlayerControl FirstKillPlayer;

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

            restrictDevices = CustomOptionHolder.RestrictDevices.GetSelection();
            restrictAdmin = CustomOptionHolder.RestrictAdmin.GetBool();
            restrictAdminTime = restrictAdminTimeMax = CustomOptionHolder.RestrictAdminTime.GetFloat();
            restrictAdminText = CustomOptionHolder.RestrictAdminText.GetBool();
            restrictCameras = CustomOptionHolder.RestrictCameras.GetBool();
            restrictCamerasTime = restrictCamerasTimeMax = CustomOptionHolder.RestrictCamerasTime.GetFloat();
            restrictCamerasText = CustomOptionHolder.RestrictCamerasText.GetBool();
            restrictVitalsText = CustomOptionHolder.RestrictVitalsText.GetBool();
            restrictVitals = CustomOptionHolder.RestrictVitals.GetBool();
            restrictVitalsTime = restrictVitalsTimeMax = CustomOptionHolder.RestrictVitalsTime.GetFloat();

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

        public static void resetDeviceTimes()
        {
            restrictAdminTime = restrictAdminTimeMax;
            restrictCamerasTime = restrictCamerasTimeMax;
            restrictVitalsTime = restrictVitalsTimeMax;
        }

        public static bool canUseAdmin
        {
            get
            {
                return restrictDevices == 0 || restrictAdminTime > 0f;
            }
        }

        public static bool couldUseAdmin
        {
            get
            {
                return restrictDevices == 0 || !restrictAdmin || restrictAdminTimeMax > 0f;
            }
        }

        public static bool canUseCameras
        {
            get
            {
                return restrictDevices == 0 || !restrictCameras || restrictCamerasTime > 0f;
            }
        }

        public static bool couldUseCameras
        {
            get
            {
                return restrictDevices == 0 || !restrictCameras || restrictCamerasTimeMax > 0f;
            }
        }

        public static bool canUseVitals
        {
            get
            {
                return restrictDevices == 0 || !restrictVitals || restrictVitalsTime > 0f;
            }
        }

        public static bool couldUseVitals
        {
            get
            {
                return restrictDevices == 0 || !restrictVitals || restrictVitalsTimeMax > 0f;
            }
        }
        public static void MeetingEndedUpdate()
        {
            ClearTimerText();
            UpdateTimerText();
        }

        public static void UpdateTimerText()
        {
            if (restrictDevices == 0 || (!restrictAdminText && !restrictCamerasText && !restrictVitalsText))
                return;
            if (FastDestroyableSingleton<HudManager>.Instance == null)
                return;

            // Admin
            if (restrictAdminText)
            {
                AdminTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                float y = -4.0f;
                if (restrictCamerasText)
                    y += 0.2f;
                if (restrictVitalsText)
                    y += 0.2f;
                AdminTimerText.transform.localPosition = new Vector3(-3.5f, y, 0);
                if (restrictAdminTime > 0)
                    AdminTimerText.text = String.Format(Tr.Get("adminText"), restrictAdminTime.ToString("0.00"));
                else
                    AdminTimerText.text = Tr.Get("adminRanOut");
                AdminTimerText.gameObject.SetActive(true);
            }

            // Cameras
            if (restrictCamerasText)
            {
                CamerasTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                float y = -4.0f;
                if (restrictVitalsText)
                    y += 0.2f;
                CamerasTimerText.transform.localPosition = new Vector3(-3.5f, y, 0);
                if (restrictCamerasTime > 0)
                    CamerasTimerText.text = String.Format(Tr.Get("camerasText"), restrictCamerasTime.ToString("0.00"));
                else
                    CamerasTimerText.text = Tr.Get("camerasRanOut");
                CamerasTimerText.gameObject.SetActive(true);
            }

            // Vitals
            if (restrictVitalsText)
            {
                VitalsTimerText = UnityEngine.Object.Instantiate(FastDestroyableSingleton<HudManager>.Instance.TaskText, FastDestroyableSingleton<HudManager>.Instance.transform);
                VitalsTimerText.transform.localPosition = new Vector3(-3.5f, -4.0f, 0);
                if (restrictVitalsTime > 0)
                    VitalsTimerText.text = String.Format(Tr.Get("vitalsText"), restrictVitalsTime.ToString("0.00"));
                else
                    VitalsTimerText.text = Tr.Get("vitalsRanOut");
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