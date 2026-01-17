using RebuildUs.Modules.Consoles;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class CounterAreaPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CounterArea), nameof(CounterArea.UpdateCount))]
    static void UpdateCountPostfix(CounterArea __instance)
    {
        Admin.UpdateCount(__instance);
    }
}