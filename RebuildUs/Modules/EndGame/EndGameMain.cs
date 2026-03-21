using Submerged.KillAnimation.Patches;
using Submerged.Systems.Oxygen;

namespace RebuildUs.Modules.EndGame;

internal static partial class EndGameMain
{
    internal static bool IsO2Win;

    internal static TMP_Text TextRenderer;
    private static readonly int Color = Shader.PropertyToID("_Color");

    internal static bool CrewmateCantWinByTaskWithoutLivingPlayer(ref bool __result)
    {
        if (CustomOptionHolder.CanWinByTaskWithoutLivingPlayer.GetBool() || Helpers.IsCrewmateAlive())
        {
            return true;
        }
        __result = false;
        return false;
    }

    internal static void OnGameEndPrefix(ref EndGameResult endGameResult)
    {
        Camouflager.ResetCamouflage();
        Morphing.ResetMorph();

        AdditionalTempData.GameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10)
        {
            endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;
        }
    }

    internal static void OnGameEndPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.LogInfo("========== GAME ENDED ==========");

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player);
            (var tasksCompleted, var tasksTotal) = TasksHandler.TaskInfo(player.Data);

            var isOxygenDeath = SubmergedCompatibility.Loaded && SubmarineOxygenSystem.Instance != null && (OxygenDeathAnimationPatches.IsOxygenDeath || IsO2Win);
            var finalStatus = GameHistory.FinalStatuses[player.PlayerId] = player.Data.Disconnected
                ? FinalStatus.Disconnected
                :
                GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out var statuse)
                    ? statuse
                    :
                    player.Data.IsDead
                        ? FinalStatus.Dead
                        :
                        isOxygenDeath && !SubmarineOxygenSystem.Instance.playersWithMask.Contains(player.PlayerId)
                            ? FinalStatus.LackOfOxygen
                            :
                            gameOverReason == GameOverReason.ImpostorsBySabotage && !player.Data.Role.IsImpostor
                                ?
                                IsO2Win ? FinalStatus.LackOfOxygen : FinalStatus.Sabotage
                                : FinalStatus.Alive;

            if (gameOverReason == GameOverReason.CrewmatesByTask && player.IsTeamCrewmate())
            {
                tasksCompleted = tasksTotal;
            }

            AdditionalTempData.PlayerRoles.Add(new()
            {
                PlayerName = player.Data.PlayerName,
                PlayerId = player.PlayerId,
                ColorId = player.Data.DefaultOutfit.ColorId,
                NameSuffix = Lovers.GetIcon(player),
                Roles = roles,
                RoleNames = RoleInfo.GetRolesString(player, true, true, excludeRoles, true),
                TasksTotal = tasksTotal,
                TasksCompleted = tasksCompleted,
                Status = finalStatus,
            });
        }

        // AdditionalTempData.IsGM = CustomOptionHolder.GmEnabled.GetBool() && PlayerControl.LocalPlayer.IsGM();

        List<PlayerControl> notWinners = [];

        notWinners.AddRange(Jackal.FormerJackals);

        if (Jester.Exists)
        {
            notWinners.Add(Jester.PlayerControl);
        }
        if (Arsonist.Exists)
        {
            notWinners.Add(Arsonist.PlayerControl);
        }
        if (Vulture.Exists)
        {
            notWinners.Add(Vulture.PlayerControl);
        }
        if (Jackal.Exists)
        {
            notWinners.Add(Jackal.PlayerControl);
        }
        if (Sidekick.Exists)
        {
            notWinners.Add(Sidekick.PlayerControl);
        }

        var sabotageWin = gameOverReason is GameOverReason.ImpostorsBySabotage;
        var impostorWin = gameOverReason is GameOverReason.ImpostorsByVote or GameOverReason.ImpostorsByKill or GameOverReason.ImpostorDisconnect;
        var crewmateWin = gameOverReason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask or GameOverReason.CrewmateDisconnect;

        // ADD HERE MORE!
        var jesterWin = Jester.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        var arsonistWin = Arsonist.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        var vultureWin = Vulture.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        var teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin;
        var miniLose = Mini.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        var loversWin = Lovers.AnyAlive() && !(Lovers.SeparateTeam && gameOverReason == GameOverReason.CrewmatesByTask);

        var everyoneDead = true;
        var playerRoles = AdditionalTempData.PlayerRoles;
        foreach (var t in playerRoles)
        {
            if (t.Status != FinalStatus.Alive)
            {
                continue;
            }
            everyoneDead = false;
            break;
        }

        var forceEnd = gameOverReason == (GameOverReason)CustomGameOverReason.ForceEnd;

        if (impostorWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.IsTeamImpostor()
                    && !p.HasModifier(ModifierType.Madmate)
                    && !p.HasModifier(ModifierType.CreatedMadmate))
                {
                    continue;
                }

                CachedPlayerData wpd = new(p.Data);
                EndGameResult.CachedWinners.Add(wpd);
            }
        }
        else if (crewmateWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.IsTeamCrewmate()
                    || p.HasModifier(ModifierType.Madmate)
                    || p.HasModifier(ModifierType.CreatedMadmate))
                {
                    continue;
                }

                CachedPlayerData wpd = new(p.Data);
                EndGameResult.CachedWinners.Add(wpd);
            }
        }

        // 勝利画面から不要なキャラを追放する
        HashSet<string> notWinnerNames = [];
        foreach (var t in notWinners)
        {
            notWinnerNames.Add(t.Data.PlayerName);
        }

        var cachedWinners = EndGameResult.CachedWinners;
        for (var i = cachedWinners.Count - 1; i >= 0; i--)
        {
            if (notWinnerNames.Contains(cachedWinners[i].PlayerName))
            {
                cachedWinners.RemoveAt(i);
            }
        }

        if (everyoneDead)
        {
            EndGameResult.CachedWinners = new();
            AdditionalTempData.WinCondition = WinCondition.EveryoneDied;
        }
        else if (jesterWin)
        {
            EndGameResult.CachedWinners = new();
            Jester.PlayerControl.Data.IsDead = true;
            EndGameResult.CachedWinners.Add(new(Jester.PlayerControl.Data));
            AdditionalTempData.WinCondition = WinCondition.JesterWin;
        }
        else if (arsonistWin)
        {
            EndGameResult.CachedWinners = new();
            EndGameResult.CachedWinners.Add(new(Arsonist.PlayerControl.Data));
            AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
        }
        else if (vultureWin)
        {
            EndGameResult.CachedWinners = new();
            EndGameResult.CachedWinners.Add(new(Vulture.PlayerControl.Data));
            AdditionalTempData.WinCondition = WinCondition.VultureWin;
        }
        else if (teamJackalWin)
        {
            // Jackal wins if nobody except jackal is alive
            AdditionalTempData.WinCondition = WinCondition.JackalWin;
            EndGameResult.CachedWinners = new();
            EndGameResult.CachedWinners.Add(new(Jackal.PlayerControl.Data)
            {
                IsImpostor = false,
            });

            // If there is a sidekick. The sidekick also wins
            EndGameResult.CachedWinners.Add(new(Sidekick.PlayerControl.Data)
            {
                IsImpostor = false,
            });

            foreach (var jackal in Jackal.FormerJackals)
            {
                EndGameResult.CachedWinners.Add(new(jackal.Data)
                {
                    IsImpostor = false,
                });
            }
        }
        // Lovers win conditions
        else if (loversWin)
        {
            // Double win for lovers, crewmates also win
            if (GameManager.Instance.DidHumansWin(gameOverReason) && !Lovers.SeparateTeam && Lovers.AnyNonKillingCouples())
            {
                AdditionalTempData.WinCondition = WinCondition.LoversTeamWin;
                AdditionalTempData.AdditionalWinConditions.Add(WinCondition.LoversTeamWin);
            }
            // Lovers solo win
            else
            {
                AdditionalTempData.WinCondition = WinCondition.LoversSoloWin;
                EndGameResult.CachedWinners = new();

                foreach (var couple in Lovers.Couples)
                {
                    if (!couple.ExistingAndAlive)
                    {
                        continue;
                    }
                    EndGameResult.CachedWinners.Add(new(couple.Lover1.Data));
                    EndGameResult.CachedWinners.Add(new(couple.Lover2.Data));
                }
            }
        }
        else if (everyoneDead)
        {
            EndGameResult.CachedWinners = new();
            AdditionalTempData.WinCondition = WinCondition.EveryoneDied;
        }

        if (forceEnd)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.Data.IsDead = false;
                EndGameResult.CachedWinners.Add(new(player.Data));
            }

            AdditionalTempData.WinCondition = WinCondition.ForceEnd;
        }

        foreach (var wpd in EndGameResult.CachedWinners.GetFastEnumerator())
        {
            var isDead = wpd.IsDead;
            if (!isDead)
            {
                foreach (var pr in playerRoles)
                {
                    if (pr.PlayerName != wpd.PlayerName || pr.Status == FinalStatus.Alive)
                    {
                        continue;
                    }
                    isDead = true;
                    break;
                }
            }

            wpd.IsDead = isDead;
        }

        RPCProcedure.ResetVariables();
    }
}