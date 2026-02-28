namespace RebuildUs;

[HarmonyPatch]
internal static class TasksHandler
{
    internal static (int Completed, int Total) TaskInfo(NetworkedPlayerInfo pInfo)
    {
        int total = 0;
        int completed = 0;
        if (pInfo.Disconnected
            || pInfo.Tasks == null
            || !pInfo.Object
            || !BoolOptionNames.GhostsDoTasks.Get() && pInfo.IsDead
            || !pInfo.Role
            || !pInfo.Role.TasksCountTowardProgress
            || pInfo.Object.IsTeamImpostor()
            || pInfo.Object.IsNeutral()
            || pInfo.Object.IsLovers() && !Lovers.HasTasks
            || pInfo.Object.HasFakeTasks())
        {
            return (completed, total);
        }

        foreach (NetworkedPlayerInfo.TaskInfo playerInfoTask in pInfo.Tasks.GetFastEnumerator())
        {
            total++;
            if (playerInfoTask.Complete)
            {
                completed++;
            }
        }

        return (completed, total);
    }

    internal static void RecomputeTaskCounts(GameData __instance)
    {
        __instance.TotalTasks = 0;
        __instance.CompletedTasks = 0;
        foreach (NetworkedPlayerInfo pInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
        {
            if (pInfo.Object
                && (pInfo.Object.IsLovers() && !Lovers.TasksCount
                    || Madmate.HasTasks && pInfo.Object?.HasModifier(ModifierType.Madmate) == true
                    || CreatedMadmate.HasTasks && pInfo.Object?.HasModifier(ModifierType.CreatedMadmate) == true
                    || MadmateRole.CanKnowImpostorAfterFinishTasks && pInfo.Object.IsRole(RoleType.Madmate)
                    || Suicider.CanKnowImpostorAfterFinishTasks && pInfo.Object.IsRole(RoleType.Suicider)))
            {
                continue;
            }

            (int playerCompleted, int playerTotal) = TaskInfo(pInfo);
            __instance.TotalTasks += playerTotal;
            __instance.CompletedTasks += playerCompleted;
        }
    }
}