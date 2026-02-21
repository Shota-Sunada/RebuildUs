namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class AlphaPulsePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AlphaPulse), nameof(AlphaPulse.Update))]
    internal static void UpdatePostfix(AlphaPulse __instance)
    {
        Map.AlphaPulseUpdate(__instance);
    }
}