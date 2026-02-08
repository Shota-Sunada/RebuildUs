namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlayerVoteAreaPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
    public static bool SelectPrefix(PlayerVoteArea __instance)
    {
        return Meeting.VoteAreaSelect();
    }
}
