namespace RebuildUs;

[HarmonyPatch]
public static class TasksHandler
{
    internal static (int Completed, int Total) TaskInfo(NetworkedPlayerInfo pInfo)
    {
        var total = 0;
        var completed = 0;
        if (pInfo.Disconnected
            || pInfo.Tasks == null
            || !pInfo.Object
            || (!Helpers.GetOption(BoolOptionNames.GhostsDoTasks) && pInfo.IsDead)
            || !pInfo.Role
            || !pInfo.Role.TasksCountTowardProgress
            || pInfo.Object.IsTeamImpostor()
            || pInfo.Object.IsNeutral()
            || pInfo.Object.IsLovers() && !Lovers.HasTasks
            || pInfo.Object.HasFakeTasks()) return (completed, total);
        foreach (var playerInfoTask in pInfo.Tasks.GetFastEnumerator())
        {
            total++;
            if (playerInfoTask.Complete) completed++;
        }

        return (completed, total);
    }

    public static void RecomputeTaskCounts(GameData gameData)
    {
        gameData.TotalTasks = 0;
        gameData.CompletedTasks = 0;
        foreach (var pInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (pInfo.Object
                && ((pInfo.Object?.IsLovers() == true && !Lovers.TasksCount)
                    || (Madmate.HasTasks && pInfo.Object?.HasModifier(ModifierType.Madmate) == true)
                    || (CreatedMadmate.HasTasks && pInfo.Object?.HasModifier(ModifierType.CreatedMadmate) == true)
                    || (MadmateRole.CanKnowImpostorAfterFinishTasks && pInfo.Object.IsRole(RoleType.Madmate))
                    || (Suicider.CanKnowImpostorAfterFinishTasks && pInfo.Object.IsRole(RoleType.Suicider))))
                continue;

            var (playerCompleted, playerTotal) = TaskInfo(pInfo);
            gameData.TotalTasks += playerTotal;
            gameData.CompletedTasks += playerCompleted;
        }
    }
}
