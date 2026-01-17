namespace RebuildUs.Patches;

[HarmonyPatch]
public static class ShipStatusPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
    public static void AwakePostfix(ShipStatus __instance)
    {
        Airship.Awake();
    }
}