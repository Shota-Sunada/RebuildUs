namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class SecurityLogGamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Awake))]
    internal static void AwakePrefix(SecurityLogGame __instance)
    {
        SecurityCamera.BeginCommon();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Update))]
    internal static bool UpdatePrefix(SecurityLogGame __instance)
    {
        return SecurityCamera.Update(__instance);
    }
}