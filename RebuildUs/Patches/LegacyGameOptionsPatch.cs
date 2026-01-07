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
}