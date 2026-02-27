namespace RebuildUs.Patches;

[HarmonyPatch]
internal static class LogicGameFlowNormalPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    internal static bool CheckEndCriteriaPrefix()
    {
        if (!GameData.Instance)
        {
            return false;
        }
        if (DestroyableSingleton<TutorialManager>.InstanceExists)
        {
            return true; // InstanceExists | Don't check Custom Criteria when in Tutorial
        }
        if (FastDestroyableSingleton<HudManager>.Instance.IsIntroDisplayed)
        {
            return false;
        }

        PlayerStatistics statistics = new();
        if (EndGameMain.CheckAndEndGameForMiniLose())
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForJesterWin())
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForArsonistWin())
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForVultureWin())
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForSabotageWin())
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForLoverWin(statistics))
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForJackalWin(statistics))
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForImpostorWin(statistics))
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForCrewmateWin(statistics))
        {
            return false;
        }
        if (EndGameMain.CheckAndEndGameForTaskWin())
        {
            return false;
        }
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    internal static void IsGameOverDueToDeathPostfix(ref bool __result)
    {
        __result = false;
    }
}