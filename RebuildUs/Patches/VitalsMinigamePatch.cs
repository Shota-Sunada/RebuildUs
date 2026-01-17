using RebuildUs.Modules.Consoles;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class VitalsMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public static void BeginPostfix(VitalsMinigame __instance)
    {
        Vitals.Begin(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    public static bool Prefix(VitalsMinigame __instance)
    {
        return Vitals.UpdatePrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    public static void Postfix(VitalsMinigame __instance)
    {
        Vitals.UpdatePostfix(__instance);
    }
}