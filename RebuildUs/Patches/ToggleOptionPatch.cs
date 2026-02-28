namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class ToggleOptionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Initialize))]
    internal static bool InitializePrefix(ToggleOption __instance)
    {
        return CustomOption.ToggleOptionInitialize(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ToggleOption), nameof(ToggleOption.Toggle))]
    internal static bool TogglePrefix(ToggleOption __instance)
    {
        CustomOption.ToggleOptionToggle(__instance);
        return false;
    }
}
