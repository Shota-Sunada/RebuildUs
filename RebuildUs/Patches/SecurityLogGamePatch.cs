namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SecurityLogGamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Awake))]
    public static void AwakePrefix(SecurityLogGame __instance)
    {
        SecurityCamera.BeginCommon();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SecurityLogGame), nameof(SecurityLogGame.Update))]
    public static bool UpdatePrefix(SecurityLogGame __instance)
    {
        return SecurityCamera.Update(__instance);
    }
}
