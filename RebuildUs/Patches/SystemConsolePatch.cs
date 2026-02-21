namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class SystemConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.CanUse))]
    internal static bool CanUsePrefix(ref float __result, SystemConsole __instance, [HarmonyArgument(0)] NetworkedPlayerInfo pc, [HarmonyArgument(1)] out bool canUse, [HarmonyArgument(2)] out bool couldUse)
    {
        canUse = couldUse = false;
        __result = float.MaxValue;
        return !Usables.IsBlocked(__instance, pc.Object);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SystemConsole), nameof(SystemConsole.Use))]
    internal static bool UsePrefix(SystemConsole __instance)
    {
        return !Usables.IsBlocked(__instance, PlayerControl.LocalPlayer);
    }
}