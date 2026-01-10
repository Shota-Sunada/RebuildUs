namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SpawnInMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
    public static void ClosePostfix()
    {
    }
}