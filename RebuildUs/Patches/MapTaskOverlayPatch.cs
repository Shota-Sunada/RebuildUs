namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MapTaskOverlayPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Show))]
    public static bool ShowPrefix(MapTaskOverlay __instance)
    {
        return Map.ShowOverlay(__instance);
    }
}
