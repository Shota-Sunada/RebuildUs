namespace RebuildUs.Patches;

[HarmonyPatch]
public static class LogicGameFlowNormalPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    public static bool CheckEndCriteriaPrefix()
    {
        if (!GameData.Instance) return false;
        if (DestroyableSingleton<TutorialManager>.InstanceExists) return true; // InstanceExists | Don't check Custom Criteria when in Tutorial
        if (FastDestroyableSingleton<HudManager>.Instance.IsIntroDisplayed) return false;

        var statistics = new PlayerStatistics();
        if (EndGameMain.CheckAndEndGameForMiniLose()) return false;
        if (EndGameMain.CheckAndEndGameForJesterWin()) return false;
        if (EndGameMain.CheckAndEndGameForArsonistWin()) return false;
        if (EndGameMain.CheckAndEndGameForVultureWin()) return false;
        if (EndGameMain.CheckAndEndGameForSabotageWin()) return false;
        if (EndGameMain.CheckAndEndGameForLoverWin(statistics)) return false;
        if (EndGameMain.CheckAndEndGameForJackalWin(statistics)) return false;
        if (EndGameMain.CheckAndEndGameForImpostorWin(statistics)) return false;
        if (EndGameMain.CheckAndEndGameForCrewmateWin(statistics)) return false;
        if (EndGameMain.CheckAndEndGameForDrawFlagWin()) return false;
        if (EndGameMain.CheckAndEndGameForRedTeamFlagWin()) return false;
        if (EndGameMain.CheckAndEndGameForBlueTeamFlagWin()) return false;
        if (EndGameMain.CheckAndEndGameForThiefModeThiefWin()) return false;
        if (EndGameMain.CheckAndEndGameForThiefModePoliceWin()) return false;
        if (EndGameMain.CheckAndEndGameForHotPotatoEnd()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyaleSoloWin()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyaleTimeWin()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyaleDraw()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyaleLimeTeamWin()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyalePinkTeamWin()) return false;
        if (EndGameMain.CheckAndEndGameForBattleRoyaleSerialKillerWin()) return false;

        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    public static void IsGameOverDueToDeathPostfix(ref bool __result)
    {
        Ship.IsGameOverDueToDeath(ref __result);
    }
}