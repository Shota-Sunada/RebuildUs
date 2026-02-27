namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameDataPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    internal static void HandleDisconnectPostfix(GameData __instance, PlayerControl player, DisconnectReasons reason)
    {
        RebuildUs.HandleDisconnect(player, reason);
        if (MeetingHud.Instance)
        {
            Meeting.SwapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
    private static bool RecomputeTaskCountsPrefix(GameData __instance)
    {
        TasksHandler.RecomputeTaskCounts(__instance);

        return false;
    }
}