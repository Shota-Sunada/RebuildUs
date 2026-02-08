namespace RebuildUs.Patches;

[HarmonyPatch]
public static class CounterAreaPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CounterArea), nameof(CounterArea.UpdateCount))]
    private static void UpdateCountPostfix(CounterArea __instance)
    {
        Admin.UpdateCount(__instance);
    }
}
