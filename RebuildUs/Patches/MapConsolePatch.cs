namespace RebuildUs.Patches;

[HarmonyPatch]
public static class MapConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.CanUse))]
    public static bool CanUsePrefix(ref float __result, MapConsole __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        canUse = couldUse = false;
        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MapConsole), nameof(MapConsole.Use))]
    public static bool UsePrefix(MapConsole __instance)
    {
        return ModMapOptions.canUseAdmin;
    }
}