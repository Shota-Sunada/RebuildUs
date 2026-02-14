namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SystemConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.CanUse))]
    public static bool CanUsePrefix(ref float __result, SystemConsole __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
    {
        canUse = couldUse = false;
        __result = float.MaxValue;
        if (Usables.IsBlocked(__instance, pc.Object)) return false;

        return true;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.Use))]
    public static bool UsePrefix(SystemConsole __instance)
    {
        return !Usables.IsBlocked(__instance, PlayerControl.LocalPlayer);
    }
}