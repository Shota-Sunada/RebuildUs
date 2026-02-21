namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ConsolePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.Use))]
    internal static bool UsePrefix(Console __instance)
    {
        return Usables.OnConsoleUse(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    internal static bool CanUsePrefix(ref float __result, Console __instance, NetworkedPlayerInfo pc, out bool canUse, out bool couldUse)
    {
        return Usables.CanUse(ref __result, __instance, pc, out canUse, out couldUse);
    }
}