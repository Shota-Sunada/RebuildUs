using RebuildUs.Modules.Consoles;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class PlanetSurveillanceMinigamePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Begin))]
    public static void BeginPrefix(PlanetSurveillanceMinigame __instance)
    {
        SecurityCamera.BeginCommon();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Update))]
    public static bool UpdatePrefix(PlanetSurveillanceMinigame __instance)
    {
        return SecurityCamera.Update(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlanetSurveillanceMinigame), nameof(PlanetSurveillanceMinigame.Close))]
    public static void ClosePrefix(PlanetSurveillanceMinigame __instance)
    {
        SecurityCamera.UseCameraTime();
    }
}