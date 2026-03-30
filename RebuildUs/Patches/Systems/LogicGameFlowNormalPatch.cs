namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class LogicGameFlowNormalPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    internal static bool CheckEndCriteriaPrefix()
    {
        return EndGameMain.CheckEndGame();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    internal static void IsGameOverDueToDeathPostfix(ref bool __result)
    {
        __result = false;
    }
}