namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlayerVoteAreaPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    internal static bool SelectPrefix(PlayerVoteArea __instance)
    {
        return Meeting.VoteAreaSelect();
    }
}