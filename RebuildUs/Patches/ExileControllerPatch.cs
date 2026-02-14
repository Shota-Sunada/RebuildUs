namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ExileControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    public static void BeginForGameplayPrefix(ExileController __instance, NetworkedPlayerInfo player, bool voteTie)
    {
        Exile.BeginForGameplay(__instance, player, voteTie);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static void WrapUpPostfix(ExileController __instance)
    {
        var networkedPlayer = __instance.initData.networkedPlayer;
        Exile.WrapUpPostfix(networkedPlayer?.Object);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.ReEnableGameplay))]
    public static void ReEnableGameplay(ExileController __instance)
    {
        Exile.ReEnableGameplay();
    }
}