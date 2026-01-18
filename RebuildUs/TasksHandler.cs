namespace RebuildUs
{
    [HarmonyPatch]
    public static class TasksHandler
    {

        public static Tuple<int, int> TaskInfo(NetworkedPlayerInfo playerInfo)
        {
            int totalTasks = 0;
            int completedTasks = 0;
            if (playerInfo != null && !playerInfo.Disconnected && playerInfo.Tasks != null &&
                playerInfo.Object &&
                playerInfo.Role && playerInfo.Role.TasksCountTowardProgress &&
                !playerInfo.Object.HasFakeTasks() && !playerInfo.Role.IsImpostor
                )
            {
                foreach (var playerInfoTask in playerInfo.Tasks.GetFastEnumerator())
                {
                    if (playerInfoTask.Complete) completedTasks++;
                    totalTasks++;
                }
            }
            return Tuple.Create(completedTasks, totalTasks);
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private static class GameDataRecomputeTaskCountsPatch
        {
            private static bool Prefix(GameData __instance)
            {

                var totalTasks = 0;
                var completedTasks = 0;

                foreach (var playerInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    // if (playerInfo.Object
                    //     && playerInfo.Object.hasAliveKillingLover() // Tasks do not count if a Crewmate has an alive killing Lover
                    //     || playerInfo.PlayerId == Lawyer.lawyer?.PlayerId // Tasks of the Lawyer do not count
                    //     || (playerInfo.PlayerId == Pursuer.pursuer?.PlayerId && Pursuer.pursuer.Data.IsDead) // Tasks of the Pursuer only count, if he's alive
                    //     || playerInfo.PlayerId == Thief.thief?.PlayerId // Thief's tasks only count after joining crew team as sheriff (and then the thief is not the thief anymore)
                    //    )
                    //     continue;
                    var (playerCompleted, playerTotal) = TaskInfo(playerInfo);
                    totalTasks += playerTotal;
                    completedTasks += playerCompleted;
                }

                __instance.TotalTasks = totalTasks;
                __instance.CompletedTasks = completedTasks;
                return false;
            }
        }
    }
}