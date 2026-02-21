using Submerged.KillAnimation.Patches;
using Submerged.Systems.Oxygen;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules.EndGame;

internal static class EndGameMain
{
    internal static bool IsO2Win;

    internal static TMP_Text TextRenderer;

    internal static bool CrewmateCantWinByTaskWithoutLivingPlayer(ref bool __result)
    {
        if (CustomOptionHolder.CanWinByTaskWithoutLivingPlayer.GetBool() || Helpers.IsCrewmateAlive()) return true;
        __result = false;
        return false;
    }

    internal static void OnGameEndPrefix(ref EndGameResult endGameResult)
    {
        Camouflager.ResetCamouflage();
        Morphing.ResetMorph();

        AdditionalTempData.GameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;
    }

    internal static void OnGameEndPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.LogInfo("Game ended.");

        GameOverReason gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            List<RoleInfo> roles = RoleInfo.GetRoleInfoForPlayer(player);
            (int tasksCompleted, int tasksTotal) = TasksHandler.TaskInfo(player.Data);

            bool isOxygenDeath = SubmergedCompatibility.Loaded && SubmarineOxygenSystem.Instance != null && (OxygenDeathAnimationPatches.IsOxygenDeath || IsO2Win);
            FinalStatus finalStatus = GameHistory.FinalStatuses[player.PlayerId] = player.Data.Disconnected
                ? FinalStatus.Disconnected
                : GameHistory.FinalStatuses.TryGetValue(player.PlayerId, out FinalStatus statuse)
                    ? statuse
                    : player.Data.IsDead
                        ? FinalStatus.Dead
                        : isOxygenDeath && !SubmarineOxygenSystem.Instance.playersWithMask.Contains(player.PlayerId)
                            ? FinalStatus.LackOfOxygen
                            : gameOverReason == GameOverReason.ImpostorsBySabotage && !player.Data.Role.IsImpostor
                                ? IsO2Win ? FinalStatus.LackOfOxygen : FinalStatus.Sabotage
                                : FinalStatus.Alive;

            if (gameOverReason == GameOverReason.CrewmatesByTask && player.IsTeamCrewmate()) tasksCompleted = tasksTotal;

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

        List<PlayerControl> notWinners = new();
        notWinners.AddRange(Jester.AllPlayers);
        notWinners.AddRange(Arsonist.AllPlayers);
        notWinners.AddRange(Vulture.AllPlayers);
        notWinners.AddRange(Jackal.AllPlayers);
        notWinners.AddRange(Sidekick.AllPlayers);
        notWinners.AddRange(Jackal.FormerJackals);

        bool sabotageWin = gameOverReason is GameOverReason.ImpostorsBySabotage;
        bool impostorWin = gameOverReason is GameOverReason.ImpostorsByVote or GameOverReason.ImpostorsByKill or GameOverReason.ImpostorDisconnect;
        bool crewmateWin = gameOverReason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask or GameOverReason.CrewmateDisconnect;

        // ADD HERE MORE!
        bool jesterWin = Jester.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        bool arsonistWin = Arsonist.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        bool vultureWin = Vulture.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        bool teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin;
        bool miniLose = Mini.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        bool loversWin = Lovers.AnyAlive() && !(Lovers.SeparateTeam && gameOverReason == GameOverReason.CrewmatesByTask);

        bool everyoneDead = true;
        List<PlayerRoleInfo> playerRoles = AdditionalTempData.PlayerRoles;
        foreach (PlayerRoleInfo t in playerRoles)
        {
            if (t.Status != FinalStatus.Alive) continue;
            everyoneDead = false;
            break;
        }

        bool forceEnd = gameOverReason == (GameOverReason)CustomGameOverReason.ForceEnd;

        if (impostorWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.IsTeamImpostor()
                    && !p.HasModifier(ModifierType.Madmate)
                    && !p.IsRole(RoleType.Madmate)
                    && !p.IsRole(RoleType.Suicider)
                    && !p.HasModifier(ModifierType.CreatedMadmate))
                    continue;

                CachedPlayerData wpd = new(p.Data);
                EndGameResult.CachedWinners.Add(wpd);
            }
        }
        else if (crewmateWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.IsTeamCrewmate()
                    || p.HasModifier(ModifierType.Madmate)
                    || p.IsRole(RoleType.Madmate)
                    || p.IsRole(RoleType.Suicider)
                    || p.HasModifier(ModifierType.CreatedMadmate))
                    continue;

                CachedPlayerData wpd = new(p.Data);
                EndGameResult.CachedWinners.Add(wpd);
            }
        }

        // 勝利画面から不要なキャラを追放する
        HashSet<string> notWinnerNames = new();
        foreach (PlayerControl t in notWinners) notWinnerNames.Add(t.Data.PlayerName);

        Il2CppSystem.Collections.Generic.List<CachedPlayerData> cachedWinners = EndGameResult.CachedWinners;
        for (int i = cachedWinners.Count - 1; i >= 0; i--)
            if (notWinnerNames.Contains(cachedWinners[i].PlayerName))
                cachedWinners.RemoveAt(i);

        if (everyoneDead)
        {
            EndGameResult.CachedWinners = new();
            AdditionalTempData.WinCondition = WinCondition.EveryoneDied;
        }
        else if (jesterWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (Jester jester in Jester.Players)
            {
                jester.Player.Data.IsDead = true;
                EndGameResult.CachedWinners.Add(new(jester.Player.Data));
            }

            AdditionalTempData.WinCondition = WinCondition.JesterWin;
        }
        else if (arsonistWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (Arsonist arsonist in Arsonist.Players) EndGameResult.CachedWinners.Add(new(arsonist.Player.Data));

            AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
        }
        else if (vultureWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (Vulture vulture in Vulture.Players) EndGameResult.CachedWinners.Add(new(vulture.Player.Data));

            AdditionalTempData.WinCondition = WinCondition.VultureWin;
        }
        else if (teamJackalWin)
        {
            // Jackal wins if nobody except jackal is alive
            AdditionalTempData.WinCondition = WinCondition.JackalWin;
            EndGameResult.CachedWinners = new();
            foreach (PlayerControl jackal in Jackal.AllPlayers) EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });

            // If there is a sidekick. The sidekick also wins
            foreach (PlayerControl sidekick in Sidekick.AllPlayers) EndGameResult.CachedWinners.Add(new(sidekick.Data) { IsImpostor = false });

            foreach (PlayerControl jackal in Jackal.FormerJackals) EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });
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

                foreach (Couple couple in Lovers.Couples)
                {
                    if (!couple.ExistingAndAlive) continue;
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
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.Data.IsDead = false;
                EndGameResult.CachedWinners.Add(new(player.Data));
            }

            AdditionalTempData.WinCondition = WinCondition.ForceEnd;
        }

        foreach (CachedPlayerData wpd in EndGameResult.CachedWinners.GetFastEnumerator())
        {
            bool isDead = wpd.IsDead;
            if (!isDead)
            {
                foreach (PlayerRoleInfo pr in playerRoles)
                {
                    if (pr.PlayerName != wpd.PlayerName || pr.Status == FinalStatus.Alive) continue;
                    isDead = true;
                    break;
                }
            }

            wpd.IsDead = isDead;
        }

        DiscordEmbedManager.SendGameResult();
        RPCProcedure.ResetVariables();
    }

    internal static void SetupEndGameScreen(EndGameManager __instance)
    {
        // Delete and readd PoolablePlayers always showing the name and role of the player
        foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>()) Object.Destroy(pb.gameObject);

        int num = Mathf.CeilToInt(7.5f);

        List<CachedPlayerData> list = new();
        Il2CppSystem.Collections.Generic.List<CachedPlayerData> cachedWinners = EndGameResult.CachedWinners;
        foreach (CachedPlayerData t in cachedWinners) list.Add(t);

        list.Sort((a, b) => (a.IsYou ? -1 : 0).CompareTo(b.IsYou ? -1 : 0));

        Dictionary<string, PlayerRoleInfo> playerRolesDict = new();
        List<PlayerRoleInfo> playerRoles = AdditionalTempData.PlayerRoles;
        foreach (PlayerRoleInfo pr in playerRoles)
            if (pr != null)
                playerRolesDict[pr.PlayerName] = pr;

        for (int i = 0; i < list.Count; i++)
        {
            CachedPlayerData cachedPlayerData2 = list[i];
            int num2 = i % 2 == 0 ? -1 : 1;
            int num3 = (i + 1) / 2;
            float num4 = num3 / (float)num;
            float num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = i == 0 ? -8 : -1;
            float num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            Vector3 vector = new(num7, num7, 1f);

            PoolablePlayer poolablePlayer = Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (num3 * 0.01f)) * 0.9f;
            poolablePlayer.transform.localScale = vector;
            if (cachedPlayerData2.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
                poolablePlayer.SetFlipX(i % 2 == 0);

            poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData2.IsDead, true);

            poolablePlayer.cosmetics.nameText.color = Color.white;
            poolablePlayer.cosmetics.nameText.lineSpacing *= 0.7f;
            poolablePlayer.cosmetics.nameText.transform.localScale = new(1f / vector.x, 1f / vector.y, 1f / vector.z);
            poolablePlayer.cosmetics.nameText.transform.localPosition = new(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y - 0.7f, -15f);

            if (playerRolesDict.TryGetValue(cachedPlayerData2.PlayerName, out PlayerRoleInfo data))
                poolablePlayer.cosmetics.nameText.text = cachedPlayerData2.PlayerName + data.NameSuffix + $"\n<size=80%>{data.RoleNames}</size>";
            else
                poolablePlayer.cosmetics.nameText.text = cachedPlayerData2.PlayerName;
        }

        // Additional code
        GameObject bonusTextObject = Object.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
        bonusTextObject.transform.localScale = new(0.7f, 0.7f, 1f);
        TextRenderer = bonusTextObject.GetComponent<TMP_Text>();
        TextRenderer.text = "";

        if (AdditionalTempData.IsGm)
        {
            __instance.WinText.text = Tr.Get(TrKey.GmGameOver);
            // __instance.WinText.color = GM.color;
        }

        string bonusText = "";

        switch (AdditionalTempData.WinCondition)
        {
            case WinCondition.JesterWin:
                bonusText = "JesterWin";
                TextRenderer.color = Jester.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.NameColor);
                break;
            case WinCondition.ArsonistWin:
                bonusText = "ArsonistWin";
                TextRenderer.color = Arsonist.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.NameColor);
                break;
            case WinCondition.VultureWin:
                bonusText = "VultureWin";
                TextRenderer.color = Vulture.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.NameColor);
                break;
            case WinCondition.JackalWin:
                bonusText = "JackalWin";
                TextRenderer.color = Jackal.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.NameColor);
                break;
            case WinCondition.MiniLose:
                bonusText = "MiniDied";
                TextRenderer.color = Mini.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.LoversTeamWin:
                bonusText = "CrewmateWin";
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
                break;
            case WinCondition.LoversSoloWin:
                bonusText = "LoversWin";
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
                break;
            case WinCondition.EveryoneDied:
                bonusText = "EveryoneDied";
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.ForceEnd:
                bonusText = "ForceEnd";
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.Default:
            default:
                switch (AdditionalTempData.GameOverReason)
                {
                    case GameOverReason.CrewmatesByTask or GameOverReason.CrewmatesByVote:
                        bonusText = "CrewmateWin";
                        TextRenderer.color = Palette.CrewmateBlue;
                        break;
                    case GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage or GameOverReason.ImpostorsByVote:
                        bonusText = "ImpostorWin";
                        TextRenderer.color = Palette.ImpostorRed;
                        break;
                }

                break;
        }

        string extraText = "";
        foreach (WinCondition w in AdditionalTempData.AdditionalWinConditions)
        {
            switch (w)
            {
                // case EWinCondition.OpportunistWin:
                //     extraText += Tr.Get(TranslateKey.opportunistExtra);
                //     break;
                case WinCondition.LoversTeamWin:
                    extraText += Tr.Get(TrKey.LoversExtra);
                    break;
            }
        }

        TextRenderer.text = extraText.Length > 0 ? string.Format(Tr.GetDynamic(bonusText + "Extra"), extraText) : Tr.GetDynamic(bonusText);

        if (MapSettings.ShowRoleSummary)
        {
            if (Camera.main != null)
            {
                Vector3 position = Camera.main.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new(1f, 1f, 1f);

                StringBuilder roleSummaryText = new();
                roleSummaryText.AppendLine(Tr.Get(TrKey.RoleSummaryText));
                AdditionalTempData.PlayerRoles.Sort((x, y) =>
                {
                    RoleInfo roleX = x.Roles.Count > 0 ? x.Roles[0] : null;
                    RoleInfo roleY = y.Roles.Count > 0 ? y.Roles[0] : null;
                    RoleType idX = roleX?.RoleType ?? RoleType.NoRole;
                    RoleType idY = roleY?.RoleType ?? RoleType.NoRole;

                    if (x.Status == y.Status) return idX == idY ? string.Compare(x.PlayerName, y.PlayerName, StringComparison.Ordinal) : idX.CompareTo(idY);

                    return x.Status.CompareTo(y.Status);
                });
                Logger.LogInfo(TextRenderer.text, "Result");
                Logger.LogInfo("----------Game Result-----------", "Result");
                foreach (PlayerRoleInfo data in AdditionalTempData.PlayerRoles)
                {
                    if (data.PlayerName == "") continue;
                    string taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>{data.TasksCompleted}/{data.TasksTotal}</color>" : "";
                    string aliveDead = Tr.GetDynamic($"{data.Status}");
                    string result = $"{data.PlayerName + data.NameSuffix}<pos=18.5%>{taskInfo}<pos=25%>{aliveDead}<pos=34%>{data.RoleNames}";
                    roleSummaryText.AppendLine(result);
                    Logger.LogInfo(result, "Result");
                }

                Logger.LogInfo("--------------------------------", "Result");

                TMP_Text roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
                roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = Color.white;
                roleSummaryTextMesh.outlineWidth *= 1.2f;
                roleSummaryTextMesh.fontSizeMin = 1.25f;
                roleSummaryTextMesh.fontSizeMax = 1.25f;
                roleSummaryTextMesh.fontSize = 1.25f;

                RectTransform roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
        }

        AdditionalTempData.Clear();
    }

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
        if (!Jester.TriggerJesterWin) return false;
        UncheckedEndGame(CustomGameOverReason.JesterWin);
        return true;
    }

    internal static bool CheckAndEndGameForArsonistWin()
    {
        if (!Arsonist.TriggerArsonistWin) return false;
        UncheckedEndGame(CustomGameOverReason.ArsonistWin);
        return true;
    }

    internal static bool CheckAndEndGameForVultureWin()
    {
        if (!Vulture.TriggerVultureWin) return false;
        UncheckedEndGame(CustomGameOverReason.VultureWin);
        return true;
    }

    internal static bool CheckAndEndGameForSabotageWin()
    {
        if (MapUtilities.Systems == null) return false;
        Dictionary<SystemTypes, Object> systems = MapUtilities.Systems;
        if (systems.TryGetValue(SystemTypes.LifeSupp, out Object systemType) && systemType != null)
        {
            LifeSuppSystemType lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType is { Countdown: < 0f })
            {
                IsO2Win = true;
                EndGameForSabotage();
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }

        if ((!systems.TryGetValue(SystemTypes.Reactor, out Object reactor) && !systems.TryGetValue(SystemTypes.Laboratory, out reactor)) || reactor == null) return false;
        ICriticalSabotage criticalSystem = reactor.TryCast<ICriticalSabotage>();
        if (criticalSystem is not { Countdown: < 0f }) return false;
        EndGameForSabotage();
        criticalSystem.ClearSabotage();
        return true;
    }

    internal static bool CheckAndEndGameForLoverWin(PlayerStatistics statistics)
    {
        if (statistics.CouplesAlive != 1 || statistics.TotalAlive > 3) return false;
        UncheckedEndGame(CustomGameOverReason.LoversWin);
        return true;
    }

    internal static bool CheckAndEndGameForJackalWin(PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive < statistics.TotalAlive - statistics.TeamJackalAlive || statistics.TeamImpostorsAlive != 0 || (statistics.TeamJackalLovers != 0 && statistics.TeamJackalLovers < statistics.CouplesAlive * 2)) return false;
        UncheckedEndGame(CustomGameOverReason.TeamJackalWin);
        return true;
    }

    internal static bool CheckAndEndGameForImpostorWin(PlayerStatistics statistics)
    {
        if (statistics.TeamImpostorsAlive < statistics.TotalAlive - statistics.TeamImpostorsAlive || statistics.TeamJackalAlive != 0 || (statistics.TeamImpostorLovers != 0 && statistics.TeamImpostorLovers < statistics.CouplesAlive * 2)) return false;
        GameOverReason endReason = GameData.LastDeathReason switch
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
        if (statistics.TeamCrew <= 0 || statistics.TeamImpostorsAlive != 0 || statistics.TeamJackalAlive != 0) return false;
        UncheckedEndGame(GameOverReason.CrewmatesByVote);
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