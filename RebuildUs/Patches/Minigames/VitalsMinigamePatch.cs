namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class VitalsMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    internal static void BeginPostfix(VitalsMinigame __instance)
    {
        Vitals.Begin(__instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    internal static bool Prefix(VitalsMinigame __instance)
    {
        return Vitals.UpdatePrefix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Update))]
    internal static void Postfix(VitalsMinigame __instance)
    {
        Vitals.UpdatePostfix(__instance);
    }
}