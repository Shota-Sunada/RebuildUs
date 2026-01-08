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
        public static bool GhostsSeeRoles = true;
        public static bool GhostsSeeModifier = true;
        public static bool GhostsSeeInformation = true;
        public static bool GhostsSeeVotes = true;
        public static bool ShowRoleSummary = true;
        public static bool AllowParallelMedBayScans = false;
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

        public static void ClearAndReloadMapOptions()
        {
            MeetingsCount = 0;
            CamerasToAdd = [];
            VentsToSeal = [];
            PlayerIcons = []; ;

            // maxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.maxNumberOfMeetings.getSelection());
            // blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.getBool();
            // noVoteIsSelfVote = CustomOptionHolder.noVoteIsSelfVote.getBool();
            // hidePlayerNames = CustomOptionHolder.hidePlayerNames.getBool();
            // allowParallelMedBayScans = CustomOptionHolder.allowParallelMedBayScans.getBool();
            // shieldFirstKill = CustomOptionHolder.shieldFirstKill.getBool();
            // firstKillPlayer = null;
        }

        public static void ReloadPluginOptions()
        {
            // ghostsSeeRoles = RebuildUs.Instance.GhostsSeeRoles.Value;
            // ghostsSeeModifier = RebuildUs.Instance.GhostsSeeModifier.Value;
            // ghostsSeeInformation = RebuildUs.Instance.GhostsSeeInformation.Value;
            // ghostsSeeVotes = RebuildUs.Instance.GhostsSeeVotes.Value;
            // showRoleSummary = RebuildUs.Instance.ShowRoleSummary.Value;
            // showLighterDarker = RebuildUs.Instance.ShowLighterDarker.Value;
            // enableSoundEffects = RebuildUs.Instance.EnableSoundEffects.Value;
            // enableHorseMode = RebuildUs.Instance.EnableHorseMode.Value;
            // ShowVentsOnMap = RebuildUs.Instance.ShowVentsOnMap.Value;
            // ShowChatNotifications = RebuildUs.Instance.ShowChatNotifications.Value;

            //Patches.ShouldAlwaysHorseAround.isHorseMode = TheOtherRolesPlugin.EnableHorseMode.Value;
        }
    }
}