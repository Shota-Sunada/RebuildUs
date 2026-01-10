namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ExileControllerPatch
{
    [HarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    public static void BeginForGameplayPrefix(ExileController __instance, [HarmonyArgument(0)] ref NetworkedPlayerInfo exiled)
    {

    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static void WrapUpPostfix(ExileController __instance)
    {
        NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
        Exile.WrapUpPostfix(networkedPlayer?.Object);
    }
}