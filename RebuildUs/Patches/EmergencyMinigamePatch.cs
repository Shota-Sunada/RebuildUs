namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class EmergencyMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    internal static void UpdatePostfix(EmergencyMinigame __instance)
    {
        Usables.EmergencyMinigameUpdate(__instance);
    }
}