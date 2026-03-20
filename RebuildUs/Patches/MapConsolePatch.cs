namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class MapConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.CanUse))]
    internal static bool CanUsePrefix(ref float __result, MapConsole __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        canUse = couldUse = false;
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.Use))]
    internal static bool UsePrefix(MapConsole __instance)
    {
        return MapSettings.CanUseAdmin;
    }
}