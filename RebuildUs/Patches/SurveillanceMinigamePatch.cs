namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SurveillanceMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    public static void BeginPrefix(SurveillanceMinigame __instance)
    {
        SecurityCamera.BeginCommon();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Begin))]
    public static void BeginPostfix(SurveillanceMinigame __instance)
    {
        SecurityCamera.BeginPostfix(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Update))]
    public static bool UpdatePrefix(SurveillanceMinigame __instance)
    {
        return SecurityCamera.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SurveillanceMinigame), nameof(SurveillanceMinigame.Close))]
    public static void ClosePrefix(SurveillanceMinigame __instance)
    {
        SecurityCamera.UseCameraTime();
    }
}