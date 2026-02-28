using HarmonyLib;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class NumberOptionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Initialize))]
    internal static bool InitializePrefix(NumberOption __instance)
    {
        return CustomOption.NumberOptionInitialize(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Increase))]
    internal static bool IncreasePrefix(NumberOption __instance)
    {
        return CustomOption.NumberOptionIncrease(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.Decrease))]
    internal static bool DecreasePrefix(NumberOption __instance)
    {
        return CustomOption.NumberOptionDecrease(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NumberOption), nameof(NumberOption.FixedUpdate))]
    internal static bool FixedUpdatePrefix(NumberOption __instance)
    {
        // Skip FixedUpdate for CustomOptions because they don't have 'data' set and will crash.
        // CustomOption handles updates via UpdateSelection.
        if (CustomOption.IsCustomOption(__instance))
        {
            return false;
        }
        return true;
    }
}
