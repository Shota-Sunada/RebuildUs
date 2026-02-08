using Submerged.Systems.Oxygen;
using Submerged.KillAnimation.Patches;

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

    public static bool IsO2Win;

    public static void OnGameEndPrefix(ref EndGameResult endGameResult)
    {
        Camouflager.ResetCamouflage();
        Morphing.ResetMorph();

        AdditionalTempData.GameOverReason = endGameResult.GameOverReason;
        if ((int)endGameResult.GameOverReason >= 10) endGameResult.GameOverReason = GameOverReason.ImpostorsByKill;
    }

    public static void OnGameEndPostfix(AmongUsClient __instance, [HarmonyArgument(0)] ref EndGameResult endGameResult)
    {
        Logger.LogInfo("Game ended.");

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player);
            var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(player.Data);

            var isOxygenDeath = SubmergedCompatibility.Loaded && SubmarineOxygenSystem.Instance != null && (OxygenDeathAnimationPatches.IsOxygenDeath || IsO2Win);
            var finalStatus = GameHistory.FinalStatuses[player.PlayerId] =
                player.Data.Disconnected == true ? FinalStatus.Disconnected :
                GameHistory.FinalStatuses.ContainsKey(player.PlayerId) ? GameHistory.FinalStatuses[player.PlayerId] :
                player.Data.IsDead == true ? FinalStatus.Dead :
                (isOxygenDeath && !SubmarineOxygenSystem.Instance.playersWithMask.Contains(player.PlayerId)) ? FinalStatus.LackOfOxygen :
                gameOverReason == GameOverReason.ImpostorsBySabotage && !player.Data.Role.IsImpostor ? (IsO2Win ? FinalStatus.LackOfOxygen : FinalStatus.Sabotage) :
                FinalStatus.Alive;

            if (gameOverReason == GameOverReason.CrewmatesByTask && player.IsTeamCrewmate()) tasksCompleted = tasksTotal;

            AdditionalTempData.PlayerRoles.Add(new PlayerRoleInfo()
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
        var jesterWin = Jester.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.JesterWin;
        var arsonistWin = Arsonist.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.ArsonistWin;
        var vultureWin = Vulture.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.VultureWin;
        var teamJackalWin = gameOverReason == (GameOverReason)CustomGameOverReason.TeamJackalWin;
        var miniLose = Mini.Exists && gameOverReason == (GameOverReason)CustomGameOverReason.MiniLose;
        var loversWin = Lovers.AnyAlive() && !(Lovers.SeparateTeam && gameOverReason == GameOverReason.CrewmatesByTask);

        var drawTeamWin = MapSettings.GameMode == CustomGameMode.CaptureTheFlag && gameOverReason == (GameOverReason)CustomGameOverReason.DrawTeamWin;
        var redTeamFlagWin = MapSettings.GameMode == CustomGameMode.CaptureTheFlag && gameOverReason == (GameOverReason)CustomGameOverReason.RedTeamFlagWin;
        var blueTeamFlagWin = MapSettings.GameMode == CustomGameMode.CaptureTheFlag && gameOverReason == (GameOverReason)CustomGameOverReason.BlueTeamFlagWin;
        var thiefModeThiefWin = MapSettings.GameMode == CustomGameMode.PoliceAndThieves && gameOverReason == (GameOverReason)CustomGameOverReason.ThiefModeThiefWin;
        var thiefModePoliceWin = MapSettings.GameMode == CustomGameMode.PoliceAndThieves && gameOverReason == (GameOverReason)CustomGameOverReason.ThiefModePoliceWin;
        var hotPotatoEnd = MapSettings.GameMode == CustomGameMode.HotPotato && gameOverReason == (GameOverReason)CustomGameOverReason.HotPotatoEnd;
        var battleRoyaleSoloWin = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyaleSoloWin;
        var battleRoyaleTimeWin = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyaleTimeWin;
        var battleRoyaleDraw = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyaleDraw;
        var battleRoyaleLimeTeamWin = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyaleLimeTeamWin;
        var battleRoyalePinkTeamWin = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyalePinkTeamWin;
        var battleRoyaleSerialKillerWin = MapSettings.GameMode == CustomGameMode.BattleRoyale && gameOverReason == (GameOverReason)CustomGameOverReason.BattleRoyaleSerialKillerWin;

        var everyoneDead = true;
        var playerRoles = AdditionalTempData.PlayerRoles;
        for (int i = 0; i < playerRoles.Count; i++)
        {
            if (playerRoles[i].Status == FinalStatus.Alive)
            {
                everyoneDead = false;
                break;
            }
        }
        var forceEnd = gameOverReason == (GameOverReason)CustomGameOverReason.ForceEnd;

        if (impostorWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.IsTeamImpostor() ||
                    p.HasModifier(ModifierType.Madmate) ||
                    p.IsRole(RoleType.Madmate) ||
                    p.IsRole(RoleType.Suicider) ||
                    p.HasModifier(ModifierType.CreatedMadmate))
                {
                    var wpd = new CachedPlayerData(p.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                }
            }
        }
        else if (crewmateWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p.IsTeamCrewmate() &&
                    !p.HasModifier(ModifierType.Madmate) &&
                    !p.IsRole(RoleType.Madmate) &&
                    !p.IsRole(RoleType.Suicider) &&
                    !p.HasModifier(ModifierType.CreatedMadmate))
                {
                    var wpd = new CachedPlayerData(p.Data);
                    EndGameResult.CachedWinners.Add(wpd);
                }
            }
        }

        // 勝利画面から不要なキャラを追放する
        var notWinnerNames = new HashSet<string>();
        for (int i = 0; i < notWinners.Count; i++)
        {
            notWinnerNames.Add(notWinners[i].Data.PlayerName);
        }

        var cachedWinners = EndGameResult.CachedWinners;
        for (int i = cachedWinners.Count - 1; i >= 0; i--)
        {
            if (notWinnerNames.Contains(cachedWinners[i].PlayerName))
            {
                cachedWinners.RemoveAt(i);
            }
        }

        if (everyoneDead)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            AdditionalTempData.WinCondition = WinCondition.EveryoneDied;
        }
        else if (jesterWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var jester in Jester.Players)
            {
                jester.Player.Data.IsDead = true;
                EndGameResult.CachedWinners.Add(new(jester.Player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.JesterWin;
        }
        else if (arsonistWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var arsonist in Arsonist.Players)
            {
                EndGameResult.CachedWinners.Add(new(arsonist.Player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
        }
        else if (vultureWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (var vulture in Vulture.Players)
            {
                EndGameResult.CachedWinners.Add(new(vulture.Player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.VultureWin;
        }
        else if (teamJackalWin)
        {
            // Jackal wins if nobody except jackal is alive
            AdditionalTempData.WinCondition = WinCondition.JackalWin;
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
                EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();

                foreach (var couple in Lovers.Couples)
                {
                    if (couple.ExistingAndAlive)
                    {
                        EndGameResult.CachedWinners.Add(new(couple.Lover1.Data));
                        EndGameResult.CachedWinners.Add(new(couple.Lover2.Data));
                    }
                }
            }
        }
        else if (everyoneDead)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            AdditionalTempData.WinCondition = WinCondition.EveryoneDied;
        }

        // Flag Game Mode Win
        // Draw
        else if (drawTeamWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.DrawTeamWin;
        }
        // Red Team Win
        else if (redTeamFlagWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in CaptureTheFlag.redteamFlag)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.RedTeamFlagWin;
        }
        // Blue Team Win
        else if (blueTeamFlagWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in CaptureTheFlag.blueteamFlag)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.BlueTeamFlagWin;
        }

        // Thief Mode Win
        // Thief Team Win
        else if (thiefModeThiefWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in PoliceAndThief.thiefTeam)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.ThiefModeThiefWin;
        }
        // Police Team Win
        else if (thiefModePoliceWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in PoliceAndThief.policeTeam)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.ThiefModePoliceWin;
        }

        // Hot Potato Game Mode Win
        else if (hotPotatoEnd)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in HotPotato.notPotatoTeamAlive)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.HotPotatoEnd;
        }

        // BattleRoyale Win
        else if (battleRoyaleSoloWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in BattleRoyale.soloPlayerTeam)
            {
                if (!player.Data.IsDead)
                {
                    EndGameResult.CachedWinners.Add(new(player.Data));
                }
            }
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleSoloWin;
        }
        // BattleRoyale Time Win
        else if (battleRoyaleTimeWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            if (BattleRoyale.matchType == 0)
            {
                foreach (PlayerControl player in BattleRoyale.soloPlayerTeam)
                {
                    if (!player.Data.IsDead)
                    {
                        EndGameResult.CachedWinners.Add(new(player.Data));
                    }
                }
            }
            else
            {
                foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (!player.Data.IsDead)
                    {
                        EndGameResult.CachedWinners.Add(new(player.Data));
                    }
                }
            }
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleTimeWin;
        }
        // BattleRoyale Lime Team Win
        else if (battleRoyaleLimeTeamWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in BattleRoyale.limeTeam)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleLimeTeamWin;
        }
        // BattleRoyale Pink Team Win
        else if (battleRoyalePinkTeamWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in BattleRoyale.pinkTeam)
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.BattleRoyalePinkTeamWin;
        }
        // BattleRoyale Serial Killer Win
        else if (battleRoyaleSerialKillerWin)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            EndGameResult.CachedWinners.Add(new(BattleRoyale.serialKiller.Data));
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleSerialKillerWin;
        }
        // BattleRoyale Draw
        else if (battleRoyaleDraw)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                EndGameResult.CachedWinners.Add(new(player.Data));
            }
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleDraw;
        }

        if (forceEnd)
        {
            EndGameResult.CachedWinners = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
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
                for (int i = 0; i < playerRoles.Count; i++)
                {
                    var pr = playerRoles[i];
                    if (pr.PlayerName == wpd.PlayerName && pr.Status != FinalStatus.Alive)
                    {
                        isDead = true;
                        break;
                    }
                }
            }
            wpd.IsDead = isDead;
        }

        DiscordEmbedManager.SendGameResult();
        RPCProcedure.ResetVariables();
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

        var list = new List<CachedPlayerData>();
        var cachedWinners = EndGameResult.CachedWinners;
        for (int i = 0; i < cachedWinners.Count; i++)
        {
            list.Add(cachedWinners[i]);
        }
        list.Sort((a, b) => (a.IsYou ? -1 : 0).CompareTo(b.IsYou ? -1 : 0));

        var playerRolesDict = new Dictionary<string, PlayerRoleInfo>();
        var playerRoles = AdditionalTempData.PlayerRoles;
        for (int i = 0; i < playerRoles.Count; i++)
        {
            var pr = playerRoles[i];
            if (pr != null) playerRolesDict[pr.PlayerName] = pr;
        }

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
            poolablePlayer.cosmetics.nameText.transform.localPosition = new Vector3(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y - 0.7f, -15f);

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
        var bonusTextObject = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new Vector3(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
        bonusTextObject.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        TextRenderer = bonusTextObject.GetComponent<TMP_Text>();
        TextRenderer.text = "";

        if (AdditionalTempData.IsGM)
        {
            __instance.WinText.text = Tr.Get(TrKey.GmGameOver);
            // __instance.WinText.color = GM.color;
        }

        string bonusText = "";
        var bonus = TrKey.None;
        var extra = TrKey.None;

        switch (AdditionalTempData.WinCondition)
        {
            case WinCondition.JesterWin:
                bonus = TrKey.JesterWin;
                extra = TrKey.JesterWinExtra;
                TextRenderer.color = Jester.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.NameColor);
                break;
            case WinCondition.ArsonistWin:
                bonus = TrKey.ArsonistWin;
                extra = TrKey.ArsonistWinExtra;
                TextRenderer.color = Arsonist.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.NameColor);
                break;
            case WinCondition.VultureWin:
                bonus = TrKey.VultureWin;
                extra = TrKey.VultureWinExtra;
                TextRenderer.color = Vulture.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.NameColor);
                break;
            case WinCondition.JackalWin:
                bonus = TrKey.JackalWin;
                extra = TrKey.JackalWinExtra;
                TextRenderer.color = Jackal.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.NameColor);
                break;
            case WinCondition.MiniLose:
                bonus = TrKey.MiniDied;
                extra = TrKey.MiniDiedExtra;
                TextRenderer.color = Mini.NameColor;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.LoversTeamWin:
                bonus = TrKey.CrewmateWin;
                extra = TrKey.CrewmateWinExtra;
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
                break;
            case WinCondition.LoversSoloWin:
                bonus = TrKey.LoversWin;
                extra = TrKey.LoversWinExtra;
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Lovers.Color);
                break;
            case WinCondition.EveryoneDied:
                bonus = TrKey.EveryoneDied;
                extra = TrKey.EveryoneDiedExtra;
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.ForceEnd:
                bonus = TrKey.ForceEnd;
                extra = TrKey.ForceEndExtra;
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.DrawTeamWin:
            case WinCondition.BattleRoyaleDraw:
                bonus = TrKey.Draw;
                extra = TrKey.DrawExtra;
                TextRenderer.color = new Color32(255, 128, 0, byte.MaxValue);
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.RedTeamFlagWin:
                bonus = TrKey.RedTeamFlagWin;
                extra = TrKey.RedTeamFlagWinExtra;
                TextRenderer.color = Color.red;
                __instance.BackgroundBar.material.SetColor("_Color", Color.red);
                break;
            case WinCondition.BlueTeamFlagWin:
                bonus = TrKey.BlueTeamFlagWin;
                extra = TrKey.BlueTeamFlagWinExtra;
                TextRenderer.color = Color.blue;
                __instance.BackgroundBar.material.SetColor("_Color", Color.blue);
                break;
            case WinCondition.ThiefModePoliceWin:
                bonus = TrKey.ThiefModePoliceWin;
                extra = TrKey.ThiefModePoliceWinExtra;
                TextRenderer.color = PoliceAndThief.PolicePlayerColor;
                __instance.BackgroundBar.material.SetColor("_Color", PoliceAndThief.PolicePlayerColor);
                break;
            case WinCondition.ThiefModeThiefWin:
                bonus = TrKey.ThiefModeThiefWin;
                extra = TrKey.ThiefModeThiefWinExtra;
                TextRenderer.color = PoliceAndThief.ThiefPlayerColor;
                __instance.BackgroundBar.material.SetColor("_Color", PoliceAndThief.ThiefPlayerColor);
                break;
            case WinCondition.HotPotatoEnd:
                bonus = TrKey.HotPotatoEnd;
                extra = TrKey.HotPotatoEndExtra;
                TextRenderer.color = Color.cyan;
                __instance.BackgroundBar.material.SetColor("_Color", Color.cyan);
                break;
            case WinCondition.BattleRoyaleSoloWin:
                bonus = TrKey.BattleRoyaleSoloWin;
                extra = TrKey.BattleRoyaleSoloWinExtra;
                TextRenderer.color = BattleRoyale.IntroColor;
                __instance.BackgroundBar.material.SetColor("_Color", BattleRoyale.IntroColor);
                break;
            case WinCondition.BattleRoyaleTimeWin:
                bonus = TrKey.BattleRoyaleTimeWin;
                extra = TrKey.BattleRoyaleTimeWinExtra;
                TextRenderer.color = BattleRoyale.IntroColor;
                __instance.BackgroundBar.material.SetColor("_Color", BattleRoyale.IntroColor);
                break;
            case WinCondition.BattleRoyaleLimeTeamWin:
                bonus = TrKey.BattleRoyaleLimeTeamWin;
                extra = TrKey.BattleRoyaleLimeTeamWinExtra;
                TextRenderer.color = BattleRoyale.LimeTeamColor;
                __instance.BackgroundBar.material.SetColor("_Color",BattleRoyale.LimeTeamColor);
                break;
            case WinCondition.BattleRoyalePinkTeamWin:
                bonus = TrKey.BattleRoyalePinkTeamWin;
                extra = TrKey.BattleRoyalePinkTeamWinExtra;
                TextRenderer.color = BattleRoyale.PinkTeamColor;
                __instance.BackgroundBar.material.SetColor("_Color", BattleRoyale.PinkTeamColor);
                break;
            case WinCondition.BattleRoyaleSerialKillerWin:
                bonus = TrKey.BattleRoyaleSerialKillerWin;
                extra = TrKey.BattleRoyaleSerialKillerWinExtra;
                TextRenderer.color = BattleRoyale.SerialKillerColor;
                __instance.BackgroundBar.material.SetColor("_Color", BattleRoyale.SerialKillerColor);
                break;
            default:
                switch (AdditionalTempData.GameOverReason)
                {
                    case GameOverReason.CrewmatesByVote:
                    case GameOverReason.CrewmatesByTask:
                        bonusText = "CrewmateWin";
                        TextRenderer.color = Palette.CrewmateBlue;
                        break;
                    case GameOverReason.ImpostorsByVote:
                    case GameOverReason.ImpostorsBySabotage:
                    case GameOverReason.ImpostorsByKill:
                        bonusText = "ImpostorWin";
                        TextRenderer.color = Palette.ImpostorRed;
                        break;
                }
                break;
        }

        string extraText = "";
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
                default:
                    break;
            }
        }

        TextRenderer.text = extraText.Length > 0
            ? string.Format(Tr.GetDynamic(bonusText + "Extra"), extraText)
            : Tr.GetDynamic(bonusText);

        if (MapSettings.ShowRoleSummary)
        {
            if (Camera.main != null)
            {
                var position = Camera.main.ViewportToWorldPoint(new Vector3(0f, 1f, Camera.main.nearClipPlane));
                GameObject roleSummary = UnityEngine.Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new Vector3(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new Vector3(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
                roleSummaryText.AppendLine(Tr.Get(TrKey.RoleSummaryText));
                AdditionalTempData.PlayerRoles.Sort((x, y) =>
                {
                    var roleX = x.Roles.Count > 0 ? x.Roles[0] : null;
                    var roleY = y.Roles.Count > 0 ? y.Roles[0] : null;
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
                    var aliveDead = Tr.GetDynamic($"{data.Status}");
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
            }

            // // webhook
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
            //         string aliveDead = Tr.GetDynamic($"{data.Status}");
            //         string result = "";
            //         result += EndGameResult.CachedWinners.ToArray().Count(x => x.PlayerName == data.PlayerName) != 0 ? ":crown: | " : ":skull: | ";
            //         result += string.Format("{0,-6} | {1,-2} | {2}", taskInfo, aliveDead, data.RoleNames);
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
            UncheckedEndGame(CustomGameOverReason.JesterWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForArsonistWin()
    {
        if (Arsonist.TriggerArsonistWin)
        {
            UncheckedEndGame(CustomGameOverReason.ArsonistWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForVultureWin()
    {
        if (Vulture.TriggerVultureWin)
        {
            UncheckedEndGame(CustomGameOverReason.VultureWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForSabotageWin()
    {
        if (MapUtilities.Systems == null) return false;
        var systems = MapUtilities.Systems;
        if (systems.TryGetValue(SystemTypes.LifeSupp, out var systemType) && systemType != null)
        {
            var lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType != null && lifeSuppSystemType.Countdown < 0f)
            {
                IsO2Win = true;
                EndGameForSabotage();
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }
        if ((systems.TryGetValue(SystemTypes.Reactor, out var reactor) || systems.TryGetValue(SystemTypes.Laboratory, out reactor)) && reactor != null)
        {
            var criticalSystem = reactor.TryCast<ICriticalSabotage>();
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
        if (statistics.CouplesAlive == 1 && statistics.TotalAlive <= 3)
        {
            UncheckedEndGame(CustomGameOverReason.LoversWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForJackalWin(PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive >= statistics.TotalAlive - statistics.TeamJackalAlive &&
            statistics.TeamImpostorsAlive == 0 &&
            (statistics.TeamJackalLovers == 0 || statistics.TeamJackalLovers >= statistics.CouplesAlive * 2)
        )
        {
            UncheckedEndGame(CustomGameOverReason.TeamJackalWin);
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
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedEndGame);
        sender.Write((byte)reason);
        sender.Write(IsO2Win);
        RPCProcedure.UncheckedEndGame((byte)reason, IsO2Win);
    }

    public static void UncheckedEndGame(CustomGameOverReason reason)
    {
        UncheckedEndGame((GameOverReason)reason);
    }

    public static bool CheckAndEndGameForDrawFlagWin()
    {
        if (CaptureTheFlag.triggerDrawWin)
        {
            UncheckedEndGame(CustomGameOverReason.DrawTeamWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForRedTeamFlagWin()
    {
        if (CaptureTheFlag.triggerRedTeamWin)
        {
            UncheckedEndGame(CustomGameOverReason.RedTeamFlagWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBlueTeamFlagWin()
    {
        if (CaptureTheFlag.triggerBlueTeamWin)
        {
            UncheckedEndGame(CustomGameOverReason.BlueTeamFlagWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForThiefModeThiefWin()
    {
        if (PoliceAndThief.triggerThiefWin)
        {
            UncheckedEndGame(CustomGameOverReason.ThiefModeThiefWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForThiefModePoliceWin()
    {
        if (PoliceAndThief.triggerPoliceWin)
        {
            UncheckedEndGame(CustomGameOverReason.ThiefModePoliceWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForHotPotatoEnd()
    {
        if (HotPotato.triggerHotPotatoEnd)
        {
            UncheckedEndGame(CustomGameOverReason.HotPotatoEnd);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyaleSoloWin()
    {
        if (BattleRoyale.triggerSoloWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyaleSoloWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyaleTimeWin()
    {
        if (BattleRoyale.triggerTimeWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyaleTimeWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyaleDraw()
    {
        if (BattleRoyale.triggerDrawWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyaleDraw);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyaleLimeTeamWin()
    {
        if (BattleRoyale.triggerLimeTeamWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyaleLimeTeamWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyalePinkTeamWin()
    {
        if (BattleRoyale.triggerPinkTeamWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyalePinkTeamWin);
            return true;
        }
        return false;
    }

    public static bool CheckAndEndGameForBattleRoyaleSerialKillerWin()
    {
        if (BattleRoyale.triggerSerialKillerWin)
        {
            UncheckedEndGame(CustomGameOverReason.BattleRoyaleSerialKillerWin);
            return true;
        }
        return false;
    }
}