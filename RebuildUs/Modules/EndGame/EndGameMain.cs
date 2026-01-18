using System.Text;

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
        Camouflager.resetCamouflage();
        Morphing.resetMorph();

        AdditionalTempData.GameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;
    }

    public static void OnGameEnd(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.LogInfo("Game ended.");

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (var player in CachedPlayer.AllPlayers)
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player.PlayerControl);
            var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(player.Data);
            var finalStatus = GameHistory.FinalStatuses[player.PlayerId] =
                player.Data.Disconnected == true ? EFinalStatus.Disconnected :
                GameHistory.FinalStatuses.ContainsKey(player.PlayerId) ? GameHistory.FinalStatuses[player.PlayerId] :
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

            var notWinners = new List<PlayerControl>();
            notWinners.AddRange(Jester.AllPlayers);
            notWinners.AddRange(Arsonist.AllPlayers);
            notWinners.AddRange(Vulture.AllPlayers);
            notWinners.AddRange(Jackal.AllPlayers);
            notWinners.AddRange(Sidekick.AllPlayers);
            notWinners.AddRange(Jackal.FormerJackals);

            var sabotageWin = gameOverReason is GameOverReason.ImpostorsBySabotage;
            var impostorWin = gameOverReason is GameOverReason.ImpostorsByVote or GameOverReason.ImpostorsByKill or GameOverReason.ImpostorDisconnect;
            var crewmateWin = gameOverReason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask or GameOverReason.CrewmateDisconnect;

            // ADD HERE MORE!
            var jesterWin = Jester.Exists && gameOverReason == (GameOverReason)ECustomGameOverReason.JesterWin;
            var arsonistWin = Arsonist.Exists && gameOverReason == (GameOverReason)ECustomGameOverReason.ArsonistWin;
            var vultureWin = Vulture.Exists && gameOverReason == (GameOverReason)ECustomGameOverReason.VultureWin;
            var teamJackalWin = gameOverReason == (GameOverReason)ECustomGameOverReason.TeamJackalWin;

            var everyoneDead = AdditionalTempData.PlayerRoles.All(x => x.Status != EFinalStatus.Alive);
            var forceEnd = gameOverReason == (GameOverReason)ECustomGameOverReason.ForceEnd;

            if (impostorWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.IsTeamImpostor() /*|| p.HasModifier(ModifierType.Madmate) || p.HasModifier(ModifierType.CreatedMadmate)*/)
                    {
                        var wpd = new CachedPlayerData(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
            }
            else if (crewmateWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var p in PlayerControl.AllPlayerControls)
                {
                    if (p.IsTeamCrewmate() /*&& !p.HasModifier(ModifierType.Madmate) && !p.HasModifier(ModifierType.CreatedMadmate)*/)
                    {
                        var wpd = new CachedPlayerData(p.Data);
                        EndGameResult.CachedWinners.Add(wpd);
                    }
                }
            }

            // 勝利画面から不要なキャラを追放する
            var winnersToRemove = new List<CachedPlayerData>();
            foreach (var winner in EndGameResult.CachedWinners)
            {
                if (notWinners.Any(x => x.Data.PlayerName == winner.PlayerName))
                {
                    winnersToRemove.Add(winner);
                }
            }
            foreach (var winner in winnersToRemove)
            {
                EndGameResult.CachedWinners.Remove(winner);
            }

            if (jesterWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var jester in Jester.Players)
                {
                    EndGameResult.CachedWinners.Add(new(jester.Player.Data));
                }
                AdditionalTempData.WinCondition = EWinCondition.JesterWin;
            }
            else if (arsonistWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var arsonist in Arsonist.Players)
                {
                    EndGameResult.CachedWinners.Add(new(arsonist.Player.Data));
                }
                AdditionalTempData.WinCondition = EWinCondition.ArsonistWin;
            }
            else if (vultureWin)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var vulture in Vulture.Players)
                {
                    EndGameResult.CachedWinners.Add(new(vulture.Player.Data));
                }
                AdditionalTempData.WinCondition = EWinCondition.VultureWin;
            }
            else if (teamJackalWin)
            {
                // Jackal wins if nobody except jackal is alive
                AdditionalTempData.WinCondition = EWinCondition.JackalWin;
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                foreach (var jackal in Jackal.AllPlayers)
                {
                    EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });
                }
                // If there is a sidekick. The sidekick also wins
                foreach (var sidekick in Sidekick.AllPlayers)
                {
                    EndGameResult.CachedWinners.Add(new(sidekick.Data) { IsImpostor = false });
                }
                foreach (var jackal in Jackal.FormerJackals)
                {
                    EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });
                }
            }
            else if (everyoneDead)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                AdditionalTempData.WinCondition = EWinCondition.EveryoneDied;
            }

            if (forceEnd)
            {
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
                AdditionalTempData.WinCondition = EWinCondition.ForceEnd;
            }

            foreach (var wpd in EndGameResult.CachedWinners)
            {
                wpd.IsDead = wpd.IsDead || AdditionalTempData.PlayerRoles.Any(x => x.PlayerName == wpd.PlayerName && x.Status != EFinalStatus.Alive);
            }

            RPCProcedure.ResetVariables();
        }
    }

    public static TMP_Text TextRenderer;

    public static void SetupEndGameScreen(EndGameManager __instance)
    {
        // Delete and readd PoolablePlayers always showing the name and role of the player
        foreach (PoolablePlayer pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
        {
            UnityEngine.Object.Destroy(pb.gameObject);
        }
        int num = Mathf.CeilToInt(7.5f);
        var list = EndGameResult.CachedWinners.ToArray().ToList().OrderBy(delegate (CachedPlayerData b)
        {
            return !b.IsYou ? 0 : -1;
        }).ToList();

        for (int i = 0; i < list.Count; i++)
        {
            var cachedPlayerData2 = list[i];
            var num2 = (i % 2 == 0) ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float)num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            var num6 = (float)((i == 0) ? -8 : -1);
            var num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            var vector = new Vector3(num7, num7, 1f);

            var poolablePlayer = UnityEngine.Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + num3 * 0.01f) * 0.9f;
            poolablePlayer.transform.localScale = vector;
            if (cachedPlayerData2.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(i % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(i % 2 == 0);
            }
            poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData2.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData2.IsDead, true);

            poolablePlayer.cosmetics.nameText.color = Color.white;
            poolablePlayer.cosmetics.nameText.lineSpacing *= 0.7f;
            poolablePlayer.cosmetics.nameText.transform.localScale = new Vector3(1f / vector.x, 1f / vector.y, 1f / vector.z);
            poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y, -15f);
            poolablePlayer.cosmetics.nameText.text = cachedPlayerData2.PlayerName;

            foreach (var data in AdditionalTempData.PlayerRoles)
            {
                if (data.PlayerName != cachedPlayerData2.PlayerName) continue;
                poolablePlayer.cosmetics.nameText.text += data.NameSuffix + $"\n<size=80%>{data.RoleNames}</size>";
            }
        }

        // Additional code
        var bonusTextObject = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
        bonusTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        TextRenderer = bonusTextObject.GetComponent<TMPro.TMP_Text>();
        TextRenderer.text = "";

        if (AdditionalTempData.IsGM)
        {
            __instance.WinText.text = Tr.Get("gmGameOver");
            // __instance.WinText.color = GM.color;
        }

        string bonusText = "";

        if (AdditionalTempData.WinCondition == EWinCondition.JesterWin)
        {
            bonusText = "jesterWin";
            TextRenderer.color = Jester.NameColor;
            __instance.BackgroundBar.material.SetColor("_Color", Jester.NameColor);
        }
        else if (AdditionalTempData.WinCondition == EWinCondition.ArsonistWin)
        {
            bonusText = "arsonistWin";
            TextRenderer.color = Arsonist.NameColor;
            __instance.BackgroundBar.material.SetColor("_Color", Arsonist.NameColor);
        }
        else if (AdditionalTempData.WinCondition == EWinCondition.VultureWin)
        {
            bonusText = "vultureWin";
            TextRenderer.color = Vulture.NameColor;
            __instance.BackgroundBar.material.SetColor("_Color", Vulture.NameColor);
        }
        // else if (AdditionalTempData.WinCondition == EWinCondition.LawyerSoloWin)
        // {
        //     bonusText = "lawyerWin";
        //     TextRenderer.color = Lawyer.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Lawyer.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.PlagueDoctorWin)
        // {
        //     bonusText = "plagueDoctorWin";
        //     TextRenderer.color = PlagueDoctor.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", PlagueDoctor.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.FoxWin)
        // {
        //     bonusText = "foxWin";
        //     TextRenderer.color = Fox.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Fox.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.PuppeteerWin)
        // {
        //     bonusText = "puppeteerWin";
        //     TextRenderer.color = Puppeteer.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Puppeteer.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.JekyllAndHydeWin)
        // {
        //     bonusText = "jekyllAndHydeWin";
        //     TextRenderer.color = JekyllAndHyde.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", JekyllAndHyde.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.MoriartyWin)
        // {
        //     bonusText = "moriartyWin";
        //     TextRenderer.color = Moriarty.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Moriarty.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.LoversTeamWin)
        // {
        //     bonusText = "crewWin";
        //     TextRenderer.color = Lovers.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.LoversSoloWin)
        // {
        //     bonusText = "loversWin";
        //     TextRenderer.color = Lovers.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Lovers.color);
        // }
        // else if (AdditionalTempData.WinCondition == EWinCondition.AkujoWin)
        // {
        //     bonusText = "akujoWin";
        //     TextRenderer.color = Akujo.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Akujo.color);
        // }
        else if (AdditionalTempData.WinCondition == EWinCondition.JackalWin)
        {
            bonusText = "jackalWin";
            TextRenderer.color = Jackal.NameColor;
            __instance.BackgroundBar.material.SetColor("_Color", Jackal.NameColor);
        }
        // else if (AdditionalTempData.WinCondition == EWinCondition.EveryoneDied)
        if (AdditionalTempData.WinCondition == EWinCondition.EveryoneDied)
        {
            bonusText = "everyoneDied";
            TextRenderer.color = Palette.DisabledGrey;
            __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
        }
        else if (AdditionalTempData.WinCondition == EWinCondition.ForceEnd)
        {
            bonusText = "forceEnd";
            TextRenderer.color = Palette.DisabledGrey;
            __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
        }
        // else if (AdditionalTempData.WinCondition == EWinCondition.MiniLose)
        // {
        //     bonusText = "miniDied";
        //     TextRenderer.color = Mini.color;
        //     __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
        // }
        else if (AdditionalTempData.GameOverReason is GameOverReason.CrewmatesByTask or GameOverReason.CrewmatesByVote)
        {
            bonusText = "crewWin";
            TextRenderer.color = Palette.White;
        }
        else if (AdditionalTempData.GameOverReason is GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage or GameOverReason.ImpostorsByVote)
        {
            bonusText = "impostorWin";
            TextRenderer.color = Palette.ImpostorRed;
        }

        string extraText = "";
        foreach (var w in AdditionalTempData.AdditionalWinConditions)
        {
            // switch (w)
            // {
            //     case EWinCondition.OpportunistWin:
            //         extraText += Tr.Get("opportunistExtra");
            //         break;
            //     case EWinCondition.LoversTeamWin:
            //         extraText += Tr.Get("loversExtra");
            //         break;
            //     case EWinCondition.AdditionalAlivePursuerWin:
            //         extraText += Tr.Get("pursuerExtra");
            //         break;
            //     default:
            //         break;
            // }
        }

        if (extraText.Length > 0)
        {
            TextRenderer.text = string.Format(Tr.Get(bonusText + "Extra"), extraText);
        }
        else
        {
            TextRenderer.text = Tr.Get(bonusText);
        }

        foreach (EWinCondition cond in AdditionalTempData.AdditionalWinConditions)
        {
            // switch (cond)
            // {
            //     case EWinCondition.AdditionalLawyerStolenWin:
            //         TextRenderer.text += $"\n{Helpers.cs(Lawyer.color, Tr.Get("lawyerExtraStolen"))}";
            //         break;
            //     case EWinCondition.AdditionalLawyerBonusWin:
            //         TextRenderer.text += $"\n{Helpers.cs(Lawyer.color, Tr.Get("lawyerExtraBonus"))}";
            //         break;
            // }
        }

        if (ModMapOptions.ShowRoleSummary)
        {
            var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
            GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
            roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
            roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

            var roleSummaryText = new StringBuilder();
            roleSummaryText.AppendLine(Tr.Get("roleSummaryText"));
            AdditionalTempData.PlayerRoles.Sort((x, y) =>
            {
                var roleX = x.Roles.FirstOrDefault();
                var roleY = y.Roles.FirstOrDefault();
                var idX = roleX == null ? RoleType.NoRole : roleX.RoleType;
                var idY = roleY == null ? RoleType.NoRole : roleY.RoleType;

                if (x.Status == y.Status)
                {
                    return idX == idY ? x.PlayerName.CompareTo(y.PlayerName) : idX.CompareTo(idY);
                }
                return x.Status.CompareTo(y.Status);

            });
            Logger.LogInfo(TextRenderer.text, "Result");
            Logger.LogInfo("----------Game Result-----------", "Result");
            foreach (var data in AdditionalTempData.PlayerRoles)
            {
                if (data.PlayerName == "") continue;
                var taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>{data.TasksCompleted}/{data.TasksTotal}</color>" : "";
                var aliveDead = Tr.Get("RoleSummary", data.Status.ToString());
                var result = $"{data.PlayerName + data.NameSuffix}<pos=18.5%>{taskInfo}<pos=25%>{aliveDead}<pos=34%>{data.RoleNames}";
                roleSummaryText.AppendLine(result);
                Logger.LogInfo(result, "Result");
            }
            Logger.LogInfo("--------------------------------", "Result");

            var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
            roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
            roleSummaryTextMesh.color = Color.white;
            roleSummaryTextMesh.outlineWidth *= 1.2f;
            roleSummaryTextMesh.fontSizeMin = 1.25f;
            roleSummaryTextMesh.fontSizeMax = 1.25f;
            roleSummaryTextMesh.fontSize = 1.25f;

            var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
            roleSummaryTextMeshRectTransform.anchoredPosition = new Vector2(position.x + 3.5f, position.y - 0.1f);
            roleSummaryTextMesh.text = roleSummaryText.ToString();

            // webhook
            // if (AmongUsClient.Instance.AmHost)
            // {
            //     List<Dictionary<string, object>> msg = [];
            //     Dictionary<string, object> embeds = [];
            //     List<Dictionary<string, object>> fields = [];
            //     foreach (var data in AdditionalTempData.PlayerRoles)
            //     {
            //         if (data.PlayerName == "") continue;
            //         // var taskInfo = data.TasksTotal > 0 ? $"{data.TasksCompleted}/{data.TasksTotal}" : "タスクなし";
            //         var taskInfo = string.Format("{0:D2}", data.TasksCompleted) + "/" + string.Format("{0:D2}", data.TasksTotal);
            //         string aliveDead = Tr.Get("roleSummary" + data.Status.ToString(), def: "-");
            //         string result = "";
            //         result += EndGameResult.CachedWinners.ToArray().Count(x => x.PlayerName == data.PlayerName) != 0 ? ":crown: | " : ":skull: | ";
            //         result += string.Format("{0,-6} | {1,-2} | {2}", taskInfo, aliveDead, data.RoleString);
            //         if (plagueExists && !data.Roles.Contains(RoleInfo.plagueDoctor))
            //         {
            //             result += " | ";
            //             if (AdditionalTempData.plagueDoctorInfected.ContainsKey(data.PlayerId))
            //             {
            //                 result += Helpers.cs(Color.red, Tr.Get("plagueDoctorInfectedText"));
            //             }
            //             else
            //             {
            //                 float progress = AdditionalTempData.plagueDoctorProgress.ContainsKey(data.PlayerId) ? AdditionalTempData.plagueDoctorProgress[data.PlayerId] : 0f;
            //                 result += PlagueDoctor.getProgressString(progress);
            //             }
            //         }
            //         Dictionary<string, object> item = new()
            //         {
            //             { "name", Webhook.colorIdToEmoji(data.ColorId) + data.PlayerName + data.NameSuffix },
            //             { "value", Regex.Replace(result, @"<[^>]*>", "") }
            //         };
            //         // item.Add("inline", true);
            //         fields.Add(item);
            //     }

            //     embeds.Add("fields", fields);
            //     msg.Add(embeds);
            //     Webhook.post(msg, bonusText, extraText);
            // }
        }
        AdditionalTempData.Clear();
    }

    public static bool CheckAndEndGameForMiniLose()
    {
        // if (Mini.triggerMiniLose)
        // {
        //     UncheckedEndGame(ECustomGameOverReason.MiniLose);
        //     return true;
        // }
        return false;
    }

    public static bool CheckAndEndGameForJesterWin()
    {
        if (Jester.TriggerJesterWin)
        {
            UncheckedEndGame(ECustomGameOverReason.JesterWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForArsonistWin()
    {
        if (Arsonist.TriggerArsonistWin)
        {
            UncheckedEndGame(ECustomGameOverReason.ArsonistWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForVultureWin()
    {
        if (Vulture.TriggerVultureWin)
        {
            UncheckedEndGame(ECustomGameOverReason.VultureWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForSabotageWin()
    {
        if (ShipStatus.Instance.Systems == null) return false;
        var systemType = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp) ? ShipStatus.Instance.Systems[SystemTypes.LifeSupp] : null;
        if (systemType != null)
        {
            var lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
            {
                EndGameForSabotage();
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }
        var systemType2 = ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor) ? ShipStatus.Instance.Systems[SystemTypes.Reactor] : null;
        systemType2 ??= ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory) ? ShipStatus.Instance.Systems[SystemTypes.Laboratory] : null;
        if (systemType2 != null)
        {
            var criticalSystem = systemType2.TryCast<ICriticalSabotage>();
            if (criticalSystem != null && criticalSystem.Countdown < 0f)
            {
                EndGameForSabotage();
                criticalSystem.ClearSabotage();
                return true;
            }
        }
        return false;
    }

    public static bool CheckAndEndGameForLoverWin(PlayerStatistics statistics)
    {
        // if (statistics.CouplesAlive == 1 && statistics.TotalAlive <= 3)
        // {
        //     UncheckedEndGame(ECustomGameOverReason.LoversWin);
        //     return true;
        // }
        return false;
    }

    public static bool CheckAndEndGameForJackalWin(PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            (statistics.TeamJackalLovers == 0 || statistics.TeamJackalLovers >= statistics.CouplesAlive * 2)
        )
        {
            UncheckedEndGame(ECustomGameOverReason.TeamJackalWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForImpostorWin(PlayerStatistics statistics)
    {
        if (statistics.TeamImpostorsAlive >= statistics.TotalAlive - statistics.TeamImpostorsAlive &&
            statistics.TeamJackalAlive == 0 &&
            (statistics.TeamImpostorLovers == 0 || statistics.TeamImpostorLovers >= statistics.CouplesAlive * 2)
           )
        {
            var endReason = GameData.LastDeathReason switch
            {
                DeathReason.Exile => GameOverReason.ImpostorsByVote,
                DeathReason.Kill => GameOverReason.ImpostorsByKill,
                _ => GameOverReason.ImpostorsByVote,
            };
            UncheckedEndGame(endReason);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForCrewmateWin(PlayerStatistics statistics)
    {
        if (statistics.TeamCrew > 0 && statistics.TeamImpostorsAlive == 0 && statistics.TeamJackalAlive == 0)
        {
            UncheckedEndGame(GameOverReason.CrewmatesByVote);
            return true;
        }
        return false;
    }

    public static void EndGameForSabotage()
    {
        UncheckedEndGame(GameOverReason.ImpostorsBySabotage);
        return;
    }

    public static void UncheckedEndGame(GameOverReason reason)
    {
        GameManager.Instance.RpcEndGame(reason, false);
        using var sender = new RPCSender(CachedPlayer.LocalPlayer.PlayerControl.NetId, CustomRPC.UncheckedEndGame);
        sender.Write((byte)reason);
        RPCProcedure.UncheckedEndGame((byte)reason);
    }

    public static void UncheckedEndGame(ECustomGameOverReason reason)
    {
        UncheckedEndGame((GameOverReason)reason);
    }
}