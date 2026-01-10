using RebuildUs.Modules;

namespace RebuildUs.Patches;

[HarmonyPatch]
public static class GameManagerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
    public static bool CheckTaskCompletionPrefix(ref bool __result)
    {
        return EndGameMain.CrewmateCantWinByTaskWithoutLivingPlayer(ref __result);
    }
}