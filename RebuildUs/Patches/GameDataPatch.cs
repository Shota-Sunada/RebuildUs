namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameDataPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
    public static void HandleDisconnectPostfix(GameData __instance, PlayerControl player, DisconnectReasons reason)
    {
        RebuildUs.HandleDisconnect(player, reason);
        if (MeetingHud.Instance) Meeting.SwapperCheckAndReturnSwap(MeetingHud.Instance, player.PlayerId);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
    private static bool RecomputeTaskCountsPrefix(GameData __instance)
    {
        TasksHandler.RecomputeTaskCounts(__instance);

        return false;
    }
}
