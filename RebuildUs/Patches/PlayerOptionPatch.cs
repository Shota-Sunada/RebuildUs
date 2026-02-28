using HarmonyLib;
using RebuildUs.Modules.CustomOptions;

namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlayerOptionPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerOption), nameof(PlayerOption.OnEnable))]
    internal static bool OnEnablePrefix(PlayerOption __instance)
    {
        return CustomOption.PlayerOptionInitialize(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerOption), nameof(PlayerOption.Increase))]
    internal static bool IncreasePrefix(PlayerOption __instance)
    {
        return CustomOption.PlayerOptionIncrease(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerOption), nameof(PlayerOption.Decrease))]
    internal static bool DecreasePrefix(PlayerOption __instance)
    {
        return CustomOption.PlayerOptionDecrease(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerOption), nameof(PlayerOption.FixedUpdate))]
    internal static bool FixedUpdatePrefix(PlayerOption __instance)
    {
        if (CustomOption.IsCustomOption(__instance))
        {
            return false;
        }
        return true;
    }
}
