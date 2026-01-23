namespace RebuildUs
{
    [HarmonyPatch]
    public static class TasksHandler
    {
        public static Tuple<int, int> TaskInfo(NetworkedPlayerInfo pInfo)
        {
            if (pInfo == null || pInfo.Disconnected || pInfo.Tasks == null || pInfo.Object == null || pInfo.Role == null || !pInfo.Role.TasksCountTowardProgress || pInfo.Object.HasFakeTasks() || pInfo.Role.IsImpostor)
                return Tuple.Create(0, 0);

            int total = 0;
            int completed = 0;
            foreach (var t in pInfo.Tasks.GetFastEnumerator())
            {
                if (t.Complete) completed++;
                total++;
            }
            return Tuple.Create(completed, total);
        }

        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private static class GameDataRecomputeTaskCountsPatch
        {
            private static bool Prefix(GameData __instance)
            {
                int total = 0;
                int completed = 0;

                foreach (var pInfo in GameData.Instance.AllPlayers.GetFastEnumerator())
                {
                    if (pInfo == null || pInfo.Disconnected || pInfo.Tasks == null || pInfo.Object == null || pInfo.Role == null || !pInfo.Role.TasksCountTowardProgress || pInfo.Object.HasFakeTasks() || pInfo.Role.IsImpostor)
                        continue;

                    foreach (var t in pInfo.Tasks.GetFastEnumerator())
                    {
                        if (t.Complete) completed++;
                        total++;
                    }
                }

                __instance.TotalTasks = total;
                __instance.CompletedTasks = completed;
                return false;
            }
        }
    }
}