namespace RebuildUs
{
    [HarmonyPatch]
    public static class TasksHandler
    {
        public static Tuple<int, int> TaskInfo(NetworkedPlayerInfo pInfo)
        {
            if (pInfo == null || pInfo.Object == null || !pInfo.Object.TasksCountTowardProgress())
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
                    if (pInfo == null || pInfo.Object == null || !pInfo.Object.TasksCountTowardProgress())
                        continue;

                    if (MadmateRole.IsRole(pInfo.Object) ||
                        Suicider.IsRole(pInfo.Object))
                    {
                        continue;
                    }

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