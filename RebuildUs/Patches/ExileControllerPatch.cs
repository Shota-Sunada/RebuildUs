namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ExileControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal static void BeginForGameplayPrefix(ExileController __instance, NetworkedPlayerInfo player, bool voteTie)
    {
        Exile.BeginForGameplay(__instance, player, voteTie);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    internal static void WrapUpPostfix(ExileController __instance)
    {
        NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
        Exile.WrapUpPostfix(networkedPlayer?.Object);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    internal static void ReEnableGameplay(ExileController __instance)
    {
        Exile.ReEnableGameplay();
    }
}