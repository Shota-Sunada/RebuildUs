namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class AirshipExileControllerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    internal static void WrapUpAndSpawnPostfix(AirshipExileController __instance)
    {
        NetworkedPlayerInfo networkedPlayer = __instance.initData.networkedPlayer;
        Exile.WrapUpPostfix(networkedPlayer?.Object);
    }
}