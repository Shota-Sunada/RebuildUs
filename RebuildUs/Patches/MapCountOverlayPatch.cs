namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MapCountOverlayPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.OnEnable))]
    public static bool OnEnablePrefix(MapCountOverlay __instance)
    {
        Admin.OnEnable(__instance);

        return Map.OverlayOnEnablePrefix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapCountOverlay), nameof(MapCountOverlay.Update))]
    public static bool UpdatePrefix(MapCountOverlay __instance)
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
