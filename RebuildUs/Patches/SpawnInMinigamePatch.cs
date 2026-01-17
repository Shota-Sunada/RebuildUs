namespace RebuildUs.Patches;

[HarmonyPatch]
public static class SpawnInMinigamePatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Close))]
    public static void ClosePostfix()
    {
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static bool BeginPrefix(SpawnInMinigame __instance, PlayerTask task)
    {
        return SpawnIn.BeginPrefix(__instance, task);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public static void BeginPostfix(SpawnInMinigame __instance)
    {
        SpawnIn.BeginPostfix(__instance);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(SpawnInMinigame._RunTimer_d__10), nameof(SpawnInMinigame._RunTimer_d__10.MoveNext))]
    public static void MoveNextPostfix(SpawnInMinigame._RunTimer_d__10 __instance)
    {
        SpawnIn.MoveNextPostfix(__instance);
    }
}