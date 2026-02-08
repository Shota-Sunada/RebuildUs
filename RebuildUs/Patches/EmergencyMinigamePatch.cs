namespace RebuildUs.Patches;

[HarmonyPatch]
public static class EmergencyMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    public static void UpdatePostfix(EmergencyMinigame __instance)
    {
        Usables.EmergencyMinigameUpdate(__instance);
    }
}
