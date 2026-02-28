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
        Logger.LogInfo("Game ended.");

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player);
            (var tasksCompleted, var tasksTotal) = TasksHandler.TaskInfo(player.Data);

            var isOxygenDeath = SubmergedCompatibility.Loaded
                                 && SubmarineOxygenSystem.Instance != null
                                 && (OxygenDeathAnimationPatches.IsOxygenDeath || IsO2Win);
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
                    && !p.IsRole(RoleType.Madmate)
                    && !p.IsRole(RoleType.Suicider)
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
                    || p.IsRole(RoleType.Madmate)
                    || p.IsRole(RoleType.Suicider)
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

    internal static void SetupEndGameScreen(EndGameManager __instance)
    {
        // Delete and readd PoolablePlayers always showing the name and role of the player
        foreach (var pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>())
        {
            UnityObject.Destroy(pb.gameObject);
        }

        var num = Mathf.CeilToInt(7.5f);

        List<CachedPlayerData> list = [];
        var cachedWinners = EndGameResult.CachedWinners;
        foreach (var t in cachedWinners)
        {
            list.Add(t);
        }

        list.Sort((a, b) => (a.IsYou ? -1 : 0).CompareTo(b.IsYou ? -1 : 0));

        Dictionary<string, PlayerRoleInfo> playerRolesDict = [];
        var playerRoles = AdditionalTempData.PlayerRoles;
        foreach (var pr in playerRoles)
        {
            if (pr != null)
            {
                playerRolesDict[pr.PlayerName] = pr;
            }
        }

        for (var i = 0; i < list.Count; i++)
        {
            var cachedPlayerData2 = list[i];
            var num2 = i % 2 == 0 ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float)num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            float num6 = i == 0 ? -8 : -1;
            var num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            Vector3 vector = new(num7, num7, 1f);

            var poolablePlayer = UnityObject.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5,
                                                         FloatRange.SpreadToEdges(-1.125f, 0f, num3, num),
                                                         num6 + num3 * 0.01f)
                                                     * 0.9f;
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

            poolablePlayer.cosmetics.nameText.color = UnityEngine.Color.white;
            poolablePlayer.cosmetics.nameText.lineSpacing *= 0.7f;
            poolablePlayer.cosmetics.nameText.transform.localScale = new(1f / vector.x, 1f / vector.y, 1f / vector.z);
            poolablePlayer.cosmetics.nameText.transform.localPosition = new(poolablePlayer.cosmetics.nameText.transform.localPosition.x,
                poolablePlayer.cosmetics.nameText.transform.localPosition.y - 0.7f,
                -15f);

            if (playerRolesDict.TryGetValue(cachedPlayerData2.PlayerName, out var data))
            {
                poolablePlayer.cosmetics.nameText.text = cachedPlayerData2.PlayerName + data.NameSuffix + $"\n<size=80%>{data.RoleNames}</size>";
            }
            else
            {
                poolablePlayer.cosmetics.nameText.text = cachedPlayerData2.PlayerName;
            }
        }

        // Additional code
        var bonusTextObject = UnityObject.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new(__instance.WinText.transform.position.x,
            __instance.WinText.transform.position.y - 0.8f,
            __instance.WinText.transform.position.z);
        bonusTextObject.transform.localScale = new(0.7f, 0.7f, 1f);
        TextRenderer = bonusTextObject.GetComponent<TMP_Text>();
        TextRenderer.text = "";

        if (AdditionalTempData.IsGm)
        {
            __instance.WinText.text = Tr.Get(TrKey.GmGameOver);
            // __instance.WinText.color = GM.color;
        }

        var bonusText = "";

        switch (AdditionalTempData.WinCondition)
        {
            case WinCondition.JesterWin:
                bonusText = "JesterWin";
                TextRenderer.color = Jester.NameColor;
                __instance.BackgroundBar.material.SetColor(Color, Jester.NameColor);
                break;
            case WinCondition.ArsonistWin:
                bonusText = "ArsonistWin";
                TextRenderer.color = Arsonist.NameColor;
                __instance.BackgroundBar.material.SetColor(Color, Arsonist.NameColor);
                break;
            case WinCondition.VultureWin:
                bonusText = "VultureWin";
                TextRenderer.color = Vulture.NameColor;
                __instance.BackgroundBar.material.SetColor(Color, Vulture.NameColor);
                break;
            case WinCondition.JackalWin:
                bonusText = "JackalWin";
                TextRenderer.color = Jackal.NameColor;
                __instance.BackgroundBar.material.SetColor(Color, Jackal.NameColor);
                break;
            case WinCondition.MiniLose:
                bonusText = "MiniDied";
                TextRenderer.color = Mini.NameColor;
                __instance.BackgroundBar.material.SetColor(Color, Palette.DisabledGrey);
                break;
            case WinCondition.LoversTeamWin:
                bonusText = "CrewmateWin";
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor(Color, Lovers.Color);
                break;
            case WinCondition.LoversSoloWin:
                bonusText = "LoversWin";
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor(Color, Lovers.Color);
                break;
            case WinCondition.EveryoneDied:
                bonusText = "EveryoneDied";
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor(Color, Palette.DisabledGrey);
                break;
            case WinCondition.ForceEnd:
                bonusText = "ForceEnd";
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor(Color, Palette.DisabledGrey);
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

        var extraText = "";
        foreach (var w in AdditionalTempData.AdditionalWinConditions)
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
                var position = Camera.main.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = UnityObject.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new(1f, 1f, 1f);

                StringBuilder roleSummaryText = new();
                roleSummaryText.AppendLine(Tr.Get(TrKey.RoleSummaryText));
                AdditionalTempData.PlayerRoles.Sort((x, y) =>
                {
                    var roleX = x.Roles.Count > 0 ? x.Roles[0] : null;
                    var roleY = y.Roles.Count > 0 ? y.Roles[0] : null;
                    var idX = roleX?.RoleType ?? RoleType.NoRole;
                    var idY = roleY?.RoleType ?? RoleType.NoRole;

                    if (x.Status == y.Status)
                    {
                        return idX == idY ? string.Compare(x.PlayerName, y.PlayerName, StringComparison.Ordinal) : idX.CompareTo(idY);
                    }

                    return x.Status.CompareTo(y.Status);
                });
                Logger.LogInfo(TextRenderer.text, "Result");
                Logger.LogInfo("----------Game Result-----------", "Result");
                foreach (var data in AdditionalTempData.PlayerRoles)
                {
                    if (data.PlayerName == "")
                    {
                        continue;
                    }
                    var taskInfo = data.TasksTotal > 0 ? $"<color=#FAD934FF>{data.TasksCompleted}/{data.TasksTotal}</color>" : "";
                    var aliveDead = Tr.GetDynamic($"{data.Status}");
                    var result = $"{data.PlayerName + data.NameSuffix}<pos=18.5%>{taskInfo}<pos=25%>{aliveDead}<pos=34%>{data.RoleNames}";
                    roleSummaryText.AppendLine(result);
                    Logger.LogInfo(result, "Result");
                }

                Logger.LogInfo("--------------------------------", "Result");

                var roleSummaryTextMesh = roleSummary.GetComponent<TMP_Text>();
                roleSummaryTextMesh.alignment = TextAlignmentOptions.TopLeft;
                roleSummaryTextMesh.color = UnityEngine.Color.white;
                roleSummaryTextMesh.outlineWidth *= 1.2f;
                roleSummaryTextMesh.fontSizeMin = 1.25f;
                roleSummaryTextMesh.fontSizeMax = 1.25f;
                roleSummaryTextMesh.fontSize = 1.25f;

                var roleSummaryTextMeshRectTransform = roleSummaryTextMesh.GetComponent<RectTransform>();
                roleSummaryTextMeshRectTransform.anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
                roleSummaryTextMesh.text = roleSummaryText.ToString();
            }
        }

        AdditionalTempData.Clear();
    }
}