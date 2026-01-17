using AmongUs.GameOptions;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class LegacyGameOptionsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LegacyGameOptions), nameof(LegacyGameOptions.AreInvalid))]
    public static bool AreInvalidPrefix(LegacyGameOptions __instance, ref int maxExpectedPlayers)
    {
        return CustomOption.LGOAreInvalid(__instance, ref maxExpectedPlayers);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LegacyGameOptions), nameof(LegacyGameOptions.Validate))]
    public static void ValidatePostfix(LegacyGameOptions __instance)
    {
        if (MapOptions.GameMode == CustomGamemodes.HideNSeek || !GameOptions.IsNormalMode) return;
        __instance.NumImpostors = GameOptions.Get(Int32OptionNames.NumImpostors);
    }
}