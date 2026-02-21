namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class GameManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
    internal static bool CheckTaskCompletionPrefix(ref bool __result)
    {
        return EndGameMain.CrewmateCantWinByTaskWithoutLivingPlayer(ref __result);
    }
}