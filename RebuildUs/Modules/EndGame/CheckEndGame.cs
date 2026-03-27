namespace RebuildUs.Modules.EndGame;

internal static partial class EndGameMain
{
    internal static bool CheckEndGame()
    {
        if (!GameData.Instance) return false;
        if (FastDestroyableSingleton<TutorialManager>.InstanceExists) return true; // InstanceExists | Don't check Custom Criteria when in Tutorial
        if (FastDestroyableSingleton<HudManager>.Instance.IsIntroDisplayed) return false;
        if (CustomOptionHolder.DontFinishGame.GetBool()) return false;

        var statistics = new PlayerStatistics();

        switch (GameModeManager.CurrentGameMode)
        {
            default:
            case CustomGamemode.Normal:
                if (CheckAndEndGameForMiniLose()) return false;
                if (CheckAndEndGameForJesterWin()) return false;
                if (CheckAndEndGameForArsonistWin()) return false;
                if (CheckAndEndGameForVultureWin()) return false;
                if (CheckAndEndGameForSabotageWin()) return false;
                if (CheckAndEndGameForLoverWin(statistics)) return false;
                if (CheckAndEndGameForJackalWin(statistics)) return false;

                if (CheckAndEndGameForImpostorWin(statistics)) return false;
                if (CheckAndEndGameForCrewmateWin(statistics)) return false;
                if (CheckAndEndGameForTaskWin()) return false;
                break;
            case CustomGamemode.BattleRoyale:
                if (BattleRoyaleMode.CheckAndEndGameForBattleRoyaleLastOne(statistics)) return false;
                if (BattleRoyaleMode.CheckAndEndGameForBattleRoyaleTimeUp()) return false;
                break;
        }

        return false;
    }
}