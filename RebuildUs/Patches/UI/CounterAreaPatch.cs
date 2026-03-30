namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class CounterAreaPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CounterArea), nameof(CounterArea.UpdateCount))]
    private static void UpdateCountPostfix(CounterArea __instance)
    {
        Admin.UpdateCount(__instance);
    }
}