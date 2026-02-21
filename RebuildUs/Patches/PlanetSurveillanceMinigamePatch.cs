namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class PlanetSurveillanceMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    internal static void BeginPrefix(PlanetSurveillanceMinigame __instance)
    {
        SecurityCamera.BeginCommon();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
    internal static bool UpdatePrefix(PlanetSurveillanceMinigame __instance)
    {
        return SecurityCamera.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Close))]
    internal static void ClosePrefix(PlanetSurveillanceMinigame __instance)
    {
        SecurityCamera.UseCameraTime();
    }
}