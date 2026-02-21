namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MapTaskOverlayPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapTaskOverlay), nameof(MapTaskOverlay.Show))]
    internal static bool ShowPrefix(MapTaskOverlay __instance)
    {
        return Map.ShowOverlay(__instance);
    }
}