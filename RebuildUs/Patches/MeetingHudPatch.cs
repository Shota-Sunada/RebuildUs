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
}