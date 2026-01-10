using RebuildUs.Modules.EndGame;
using RebuildUs.Players;
using RebuildUs.Roles;

namespace RebuildUs.Modules;

public static class EndGameMain
{
    public static bool CrewmateCantWinByTaskWithoutLivingPlayer(ref bool __result)
    {
        if (!CustomOptionHolder.CanWinByTaskWithoutLivingPlayer.GetBool() && !Helpers.IsCrewmateAlive())
        {
            __result = false;
            return false;
        }
        return true;
    }

    public static void OnGameEndPrefix(ref EndGameResult endGameResult)
    {
        // Camouflager.resetCamouflage();
        // Morphling.resetMorph();

        AdditionalTempData.GameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;
    }

    public static void OnGameEnd(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.LogInfo("Game ended.");

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        ERoleType[] excludeRoles = [];
        foreach (var player in CachedPlayer.AllPlayers)
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player.PlayerControl);
            var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(player.Data);
            var finalStatus = GameHistory.finalStatuses[player.PlayerId] =
                player.Data.Disconnected == true ? EFinalStatus.Disconnected :
                GameHistory.finalStatuses.ContainsKey(player.PlayerId) ? GameHistory.finalStatuses[player.PlayerId] :
                player.Data.IsDead == true ? EFinalStatus.Dead :
                gameOverReason == GameOverReason.ImpostorsBySabotage && !player.Data.Role.IsImpostor ? EFinalStatus.Sabotage :
                EFinalStatus.Alive;

            if (gameOverReason == GameOverReason.CrewmatesByTask && player.Data.Object.IsTeamCrewmate()) tasksCompleted = tasksTotal;

            AdditionalTempData.PlayerRoles.Add(new PlayerRoleInfo()
            {
                PlayerName = player.Data.PlayerName,
                PlayerId = player.PlayerId,
                ColorId = player.Data.DefaultOutfit.ColorId,
                // NameSuffix = Lovers.getIcon(p.Object) + Cupid.getIcon(p.Object) + Akujo.getIcon(p.Object),
                Roles = roles,
                RoleNames = RoleInfo.GetRolesString(player.Data.Object, true, true, excludeRoles, true),
                TasksTotal = tasksTotal,
                TasksCompleted = tasksCompleted,
                Status = finalStatus,
            });

            // AdditionalTempData.IsGM = CustomOptionHolder.GmEnabled.GetBool() && CachedPlayer.LocalPlayer.PlayerControl.IsGM();
            // AdditionalTempData.plagueDoctorInfected = PlagueDoctor.infected;
            // AdditionalTempData.plagueDoctorProgress = PlagueDoctor.progress;
        }
    }
}