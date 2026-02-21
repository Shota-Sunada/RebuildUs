namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class LegacyGameOptionsPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LegacyGameOptions), nameof(LegacyGameOptions.AreInvalid))]
    internal static bool AreInvalidPrefix(LegacyGameOptions __instance, ref int maxExpectedPlayers)
    {
        return CustomOption.LgoAreInvalid(__instance, ref maxExpectedPlayers);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LegacyGameOptions), nameof(LegacyGameOptions.Validate))]
    internal static void ValidatePostfix(LegacyGameOptions __instance)
    {
        if (!Helpers.IsNormal) return;
        __instance.NumImpostors = Helpers.GetOption(Int32OptionNames.NumImpostors);
    }
}