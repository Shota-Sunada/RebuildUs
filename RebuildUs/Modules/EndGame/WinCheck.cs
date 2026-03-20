namespace RebuildUs.Modules.EndGame;

internal static partial class EndGameMain
{
    internal static bool CheckAndEndGameForMiniLose()
    {
        // if (Mini.triggerMiniLose)
        // {
        //     UncheckedEndGame(ECustomGameOverReason.MiniLose);
        //     return true;
        // }
        return false;
    }

    internal static bool CheckAndEndGameForJesterWin()
    {
        if (!Jester.TriggerJesterWin)
        {
            return false;
        }
        UncheckedEndGame(CustomGameOverReason.JesterWin);
        return true;
    }

    internal static bool CheckAndEndGameForArsonistWin()
    {
        if (!Arsonist.TriggerArsonistWin)
        {
            return false;
        }
        UncheckedEndGame(CustomGameOverReason.ArsonistWin);
        return true;
    }

    internal static bool CheckAndEndGameForVultureWin()
    {
        if (!Vulture.TriggerVultureWin)
        {
            return false;
        }
        UncheckedEndGame(CustomGameOverReason.VultureWin);
        return true;
    }

    internal static bool CheckAndEndGameForSabotageWin()
    {
        if (MapUtilities.Systems == null)
        {
            return false;
        }
        var systems = MapUtilities.Systems;
        if (systems.TryGetValue(SystemTypes.LifeSupp, out var systemType) && systemType != null)
        {
            var lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType is
                {
                    Countdown: < 0f,
                })
            {
                IsO2Win = true;
                EndGameForSabotage();
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }

        if (!systems.TryGetValue(SystemTypes.Reactor, out var reactor) && !systems.TryGetValue(SystemTypes.Laboratory, out reactor)
            || reactor == null)
        {
            return false;
        }
        var criticalSystem = reactor.TryCast<ICriticalSabotage>();
        if (criticalSystem is not
            {
                Countdown: < 0f,
            })
        {
            return false;
        }
        EndGameForSabotage();
        criticalSystem.ClearSabotage();
        return true;
    }

    internal static bool CheckAndEndGameForLoverWin(PlayerStatistics statistics)
    {
        if (statistics.CouplesAlive != 1 || statistics.TotalAlive > 3)
        {
            return false;
        }
        UncheckedEndGame(CustomGameOverReason.LoversWin);
        return true;
    }

    internal static bool CheckAndEndGameForJackalWin(PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive < statistics.TotalAlive - statistics.TeamJackalAlive
            || statistics.TeamImpostorsAlive != 0
            || statistics.TeamJackalLovers != 0 && statistics.TeamJackalLovers < statistics.CouplesAlive * 2)
        {
            return false;
        }

        UncheckedEndGame(CustomGameOverReason.TeamJackalWin);
        return true;
    }

    internal static bool CheckAndEndGameForImpostorWin(PlayerStatistics statistics)
    {
        if (statistics.TeamImpostorsAlive < statistics.TotalAlive - statistics.TeamImpostorsAlive
            || statistics.TeamJackalAlive != 0
            || statistics.TeamImpostorLovers != 0 && statistics.TeamImpostorLovers < statistics.CouplesAlive * 2)
        {
            return false;
        }

        var endReason = GameData.LastDeathReason switch
        {
            DeathReason.Exile => GameOverReason.ImpostorsByVote,
            DeathReason.Kill => GameOverReason.ImpostorsByKill,
            _ => GameOverReason.ImpostorsByVote,
        };
        UncheckedEndGame(endReason);
        return true;
    }

    internal static bool CheckAndEndGameForCrewmateWin(PlayerStatistics statistics)
    {
        if (statistics.TeamCrew <= 0 || statistics.TeamImpostorsAlive != 0 || statistics.TeamJackalAlive != 0)
        {
            return false;
        }

        UncheckedEndGame(GameOverReason.CrewmatesByVote);
        return true;
    }

    internal static bool CheckAndEndGameForTaskWin()
    {
        if (GameData.Instance.TotalTasks <= 0 || GameData.Instance.TotalTasks > GameData.Instance.CompletedTasks)
        {
            return false;
        }

        UncheckedEndGame(GameOverReason.CrewmatesByTask);
        return true;
    }

    private static void EndGameForSabotage()
    {
        UncheckedEndGame(GameOverReason.ImpostorsBySabotage);
    }

    private static void UncheckedEndGame(GameOverReason reason)
    {
        GameManager.Instance.RpcEndGame(reason, false);
        using RPCSender sender = new(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedEndGame);
        sender.Write((byte)reason);
        sender.Write(IsO2Win);
        RPCProcedure.UncheckedEndGame((byte)reason, IsO2Win);
    }

    private static void UncheckedEndGame(CustomGameOverReason reason)
    {
        UncheckedEndGame((GameOverReason)reason);
    }
}