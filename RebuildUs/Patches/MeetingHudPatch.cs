namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MeetingHudPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
    public static bool CheckForEndVotingPrefix(MeetingHud __instance)
    {
        return Meeting.CheckForEndVoting(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.BloopAVoteIcon))]
    public static bool BloopAVoteIconPrefix(MeetingHud __instance, NetworkedPlayerInfo voterPlayer, int index, Transform parent)
    {
        return Meeting.BloopAVoteIcon(__instance, voterPlayer, index, parent);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
    public static bool PopulateResultsPrefix(MeetingHud __instance, Il2CppStructArray<MeetingHud.VoterState> states)
    {
        return Meeting.PopulateVotes(__instance, states);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static void UpdatePostfix(MeetingHud __instance)
    {
        if (MapOptions.BlockSkippingInEmergencyMeetings)
        {
            __instance.SkipVoteButton?.gameObject?.SetActive(false);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
    public static bool SelectPrefix(ref bool __result, MeetingHud __instance, [HarmonyArgument(0)] int suspectStateIdx)
    {
        __result = false;
        // if (GM.gm != null && GM.gm.PlayerId == suspectStateIdx) return false;
        if (MapOptions.NoVoteIsSelfVote && CachedPlayer.LocalPlayer.PlayerControl.PlayerId == suspectStateIdx) return false;
        if (MapOptions.BlockSkippingInEmergencyMeetings && suspectStateIdx == -1) return false;

        return true;
    }
}