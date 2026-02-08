namespace RebuildUs.Patches;

[HarmonyPatch]
public static class NormalGameOptionsV10Patch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NormalGameOptionsV10), nameof(NormalGameOptionsV10.AreInvalid))]
    public static bool AreInvalidPrefix(NormalGameOptionsV10 __instance, ref int maxExpectedPlayers)
    {
        return CustomOption.Ngo10AreInvalid(__instance, ref maxExpectedPlayers);
    }
}
