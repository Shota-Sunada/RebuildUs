namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class NormalGameOptionsV10Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NormalGameOptionsV10), nameof(NormalGameOptionsV10.AreInvalid))]
    internal static bool AreInvalidPrefix(NormalGameOptionsV10 __instance, ref int maxExpectedPlayers)
    {
        return CustomOption.Ngo10AreInvalid(__instance, ref maxExpectedPlayers);
    }
}