namespace RebuildUs.Patches;

[HarmonyPatch]
public static class AirshipExileControllerPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static void WrapUpAndSpawnPostfix(AirshipExileController __instance)
    {
        var networkedPlayer = __instance.initData.networkedPlayer;
        Exile.WrapUpPostfix(networkedPlayer?.Object);
    }
}