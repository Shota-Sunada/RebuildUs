namespace RebuildUs.Patches;

[HarmonyPatch]
public static class AlphaPulsePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AlphaPulse), nameof(AlphaPulse.Update))]
    public static void UpdatePostfix(AlphaPulse __instance)
    {
        Map.AlphaPulseUpdate(__instance);
    }
}