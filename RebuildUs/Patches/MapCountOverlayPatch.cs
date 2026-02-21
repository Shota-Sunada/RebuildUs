namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MapCountOverlayPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnEnable))]
    internal static bool OnEnablePrefix(MapCountOverlay __instance)
    {
        Admin.OnEnable(__instance);

        return Map.OverlayOnEnablePrefix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    internal static bool UpdatePrefix(MapCountOverlay __instance)
    {
        Admin.Update(__instance);

        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnDisable))]
    private static void OnDisablePrefix(MapCountOverlay __instance)
    {
        Admin.OnDisable();
    }
}