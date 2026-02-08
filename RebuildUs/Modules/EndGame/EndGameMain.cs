using Submerged.KillAnimation.Patches;
using Submerged.Systems.Oxygen;
using Object = UnityEngine.Object;

namespace RebuildUs.Modules;

internal static class EndGameMain
{
    internal static bool IsO2Win;

    internal static TMP_Text TextRenderer;
    private static readonly int ColorID = Shader.PropertyToID("_Color");

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

        var gameOverReason = AdditionalTempData.GameOverReason;
        AdditionalTempData.Clear();

        RoleType[] excludeRoles = [];
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            var roles = RoleInfo.GetRoleInfoForPlayer(player);
            var (tasksCompleted, tasksTotal) = TasksHandler.TaskInfo(player.Data);

            var isOxygenDeath = SubmergedCompatibility.Loaded && SubmarineOxygenSystem.Instance != null && (OxygenDeathAnimationPatches.IsOxygenDeath || IsO2Win);
            var finalStatus = GameHistory.FINAL_STATUSES[player.PlayerId] = player.Data.Disconnected
                ? FinalStatus.Disconnected
                : GameHistory.FINAL_STATUSES.TryGetValue(player.PlayerId, out var value)
                    ? value
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
        foreach (var t in playerRoles)
        {
            if (t.Status != FinalStatus.Alive) continue;
            everyoneDead = false;
            break;
        }

        var forceEnd = gameOverReason == (GameOverReason)CustomGameOverReason.ForceEnd;

        // 勝利画面から不要なキャラを追放する
        var notWinnerNames = new HashSet<string>();
        foreach (var t in notWinners) notWinnerNames.Add(t.Data.PlayerName);

        var cachedWinners = EndGameResult.CachedWinners;
        for (var i = 0; i < cachedWinners.Count; i++)
        {
            if (notWinnerNames.Contains(cachedWinners[i].PlayerName))
            {
                cachedWinners.RemoveAt(i);
            }
        }

        if (impostorWin || sabotageWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (!p.IsTeamImpostor()
                    && !p.HasModifier(ModifierType.Madmate)
                    && !p.IsRole(RoleType.Madmate)
                    && !p.IsRole(RoleType.Suicider)
                    && !p.HasModifier(ModifierType.CreatedMadmate)) continue;

                var wpd = new CachedPlayerData(p.Data);
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
                    || p.HasModifier(ModifierType.CreatedMadmate)) continue;

                var wpd = new CachedPlayerData(p.Data);
                EndGameResult.CachedWinners.Add(wpd);
            }
        }
        else if (jesterWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var jester in Jester.Players)
            {
                jester.Player.Data.IsDead = true;
                EndGameResult.CachedWinners.Add(new(jester.Player.Data) { IsDead = true });
            }

            AdditionalTempData.WinCondition = WinCondition.JesterWin;
        }
        else if (arsonistWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var arsonist in Arsonist.Players) EndGameResult.CachedWinners.Add(new(arsonist.Player.Data));
            AdditionalTempData.WinCondition = WinCondition.ArsonistWin;
        }
        else if (vultureWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var vulture in Vulture.Players) EndGameResult.CachedWinners.Add(new(vulture.Player.Data));
            AdditionalTempData.WinCondition = WinCondition.VultureWin;
        }
        else if (teamJackalWin)
        {
            // Jackal wins if nobody except jackal is alive
            AdditionalTempData.WinCondition = WinCondition.JackalWin;
            EndGameResult.CachedWinners = new();
            foreach (var jackal in Jackal.AllPlayers) EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });
            // If there is a sidekick. The sidekick also wins
            foreach (var sidekick in Sidekick.AllPlayers) EndGameResult.CachedWinners.Add(new(sidekick.Data) { IsImpostor = false });
            foreach (var jackal in Jackal.FormerJackals) EndGameResult.CachedWinners.Add(new(jackal.Data) { IsImpostor = false });
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

        // Flag Game Mode Win
        // Draw
        else if (drawTeamWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator()) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.DrawTeamWin;
        }
        // Red Team Win
        else if (redTeamFlagWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in CaptureTheFlag.RedteamFlag) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.RedTeamFlagWin;
        }
        // Blue Team Win
        else if (blueTeamFlagWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in CaptureTheFlag.BlueteamFlag) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.BlueTeamFlagWin;
        }

        // Thief Mode Win
        // Thief Team Win
        else if (thiefModeThiefWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PoliceAndThief.ThiefTeam) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.ThiefModeThiefWin;
        }
        // Police Team Win
        else if (thiefModePoliceWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PoliceAndThief.PoliceTeam) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.ThiefModePoliceWin;
        }

        // Hot Potato Game Mode Win
        else if (hotPotatoEnd)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in HotPotato.NOT_POTATO_TEAM_ALIVE) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.HotPotatoEnd;
        }

        // BattleRoyale Win
        else if (battleRoyaleSoloWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in BattleRoyale.SoloPlayerTeam)
            {
                if (!player.Data.IsDead) EndGameResult.CachedWinners.Add(new(player.Data));
            }

            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleSoloWin;
        }
        // BattleRoyale Time Win
        else if (battleRoyaleTimeWin)
        {
            EndGameResult.CachedWinners = new();
            if (BattleRoyale.MatchType == 0)
            {
                foreach (var player in BattleRoyale.SoloPlayerTeam)
                {
                    if (!player.Data.IsDead) EndGameResult.CachedWinners.Add(new(player.Data));
                }
            }
            else
            {
                foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (!player.Data.IsDead) EndGameResult.CachedWinners.Add(new(player.Data));
                }
            }

            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleTimeWin;
        }
        // BattleRoyale Lime Team Win
        else if (battleRoyaleLimeTeamWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in BattleRoyale.LimeTeam) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleLimeTeamWin;
        }
        // BattleRoyale Pink Team Win
        else if (battleRoyalePinkTeamWin)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in BattleRoyale.PinkTeam) EndGameResult.CachedWinners.Add(new(player.Data));
            AdditionalTempData.WinCondition = WinCondition.BattleRoyalePinkTeamWin;
        }
        // BattleRoyale Serial Killer Win
        else if (battleRoyaleSerialKillerWin)
        {
            EndGameResult.CachedWinners = new();
            EndGameResult.CachedWinners.Add(new(BattleRoyale.SerialKiller.Data));
            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleSerialKillerWin;
        }
        // BattleRoyale Draw
        else if (battleRoyaleDraw)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.Data.IsDead = false;
                EndGameResult.CachedWinners.Add(new(player.Data) { IsDead = false });
            }

            AdditionalTempData.WinCondition = WinCondition.BattleRoyaleDraw;
        }

        if (forceEnd)
        {
            EndGameResult.CachedWinners = new();
            foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                player.Data.IsDead = false;
                EndGameResult.CachedWinners.Add(new(player.Data) { IsDead = false });
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

    public static void SetupEndGameScreen(EndGameManager __instance)
    {
        // Delete and readd PoolablePlayers always showing the name and role of the player
        foreach (var pb in __instance.transform.GetComponentsInChildren<PoolablePlayer>()) Object.Destroy(pb.gameObject);
        var num = Mathf.CeilToInt(7.5f);

        var list = new List<CachedPlayerData>();
        var cachedWinners = EndGameResult.CachedWinners;
        foreach (var t in cachedWinners) list.Add(t);

        list.Sort((a, b) => (a.IsYou ? -1 : 0).CompareTo(b.IsYou ? -1 : 0));

        var playerRolesDict = new Dictionary<string, PlayerRoleInfo>();
        var playerRoles = AdditionalTempData.PlayerRoles;
        foreach (var pr in playerRoles)
        {
            if (pr != null) playerRolesDict[pr.PlayerName] = pr;
        }

        for (var i = 0; i < list.Count; i++)
        {
            var cachedPlayerData2 = list[i];
            var num2 = i % 2 == 0 ? -1 : 1;
            var num3 = (i + 1) / 2;
            var num4 = num3 / (float)num;
            var num5 = Mathf.Lerp(1f, 0.75f, num4);
            var num6 = (float)(i == 0 ? -8 : -1);
            var num7 = Mathf.Lerp(1f, 0.65f, num4) * 0.9f;
            var vector = new Vector3(num7, num7, 1f);

            var poolablePlayer = Object.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num2 * num3 * num5, FloatRange.SpreadToEdges(-1.125f, 0f, num3, num), num6 + (num3 * 0.01f)) * 0.9f;
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
            poolablePlayer.cosmetics.nameText.transform.localScale = new(1f / vector.x, 1f / vector.y, 1f / vector.z);
            poolablePlayer.cosmetics.nameText.transform.localPosition = new(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y - 0.7f, -15f);

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
        var bonusTextObject = Object.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
        bonusTextObject.transform.localScale = new(0.7f, 0.7f, 1f);
        TextRenderer = bonusTextObject.GetComponent<TMP_Text>();
        TextRenderer.text = "";

        if (AdditionalTempData.IsGm)
        {
            __instance.WinText.text = Tr.Get(TrKey.GmGameOver);
            // __instance.WinText.color = GM.color;
        }

        var bonus = TrKey.None;
        var extra = TrKey.None;

        switch (AdditionalTempData.WinCondition)
        {
            case WinCondition.JesterWin:
                bonus = TrKey.JesterWin;
                extra = TrKey.JesterWinExtra;
                TextRenderer.color = Jester.NameColor;
                __instance.BackgroundBar.material.SetColor(ColorID, Jester.NameColor);
                break;
            case WinCondition.ArsonistWin:
                bonus = TrKey.ArsonistWin;
                extra = TrKey.ArsonistWinExtra;
                TextRenderer.color = Arsonist.NameColor;
                __instance.BackgroundBar.material.SetColor(ColorID, Arsonist.NameColor);
                break;
            case WinCondition.VultureWin:
                bonus = TrKey.VultureWin;
                extra = TrKey.VultureWinExtra;
                TextRenderer.color = Vulture.NameColor;
                __instance.BackgroundBar.material.SetColor(ColorID, Vulture.NameColor);
                break;
            case WinCondition.JackalWin:
                bonus = TrKey.JackalWin;
                extra = TrKey.JackalWinExtra;
                TextRenderer.color = Jackal.NameColor;
                __instance.BackgroundBar.material.SetColor(ColorID, Jackal.NameColor);
                break;
            case WinCondition.MiniLose:
                bonus = TrKey.MiniDied;
                extra = TrKey.MiniDiedExtra;
                TextRenderer.color = Mini.NameColor;
                __instance.BackgroundBar.material.SetColor(ColorID, Palette.DisabledGrey);
                break;
            case WinCondition.LoversTeamWin:
                bonus = TrKey.CrewmateWin;
                extra = TrKey.CrewmateWinExtra;
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor(ColorID, Lovers.Color);
                break;
            case WinCondition.LoversSoloWin:
                bonus = TrKey.LoversWin;
                extra = TrKey.LoversWinExtra;
                TextRenderer.color = Lovers.Color;
                __instance.BackgroundBar.material.SetColor(ColorID, Lovers.Color);
                break;
            case WinCondition.EveryoneDied:
                bonus = TrKey.EveryoneDied;
                extra = TrKey.EveryoneDiedExtra;
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor(ColorID, Palette.DisabledGrey);
                break;
            case WinCondition.ForceEnd:
                bonus = TrKey.ForceEnd;
                extra = TrKey.ForceEndExtra;
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor(ColorID, Palette.DisabledGrey);
                break;
            case WinCondition.DrawTeamWin:
            case WinCondition.BattleRoyaleDraw:
                bonus = TrKey.Draw;
                extra = TrKey.DrawExtra;
                TextRenderer.color = new Color32(255, 128, 0, byte.MaxValue);
                __instance.BackgroundBar.material.SetColor(ColorID, Palette.DisabledGrey);
                break;
            case WinCondition.RedTeamFlagWin:
                bonus = TrKey.RedTeamFlagWin;
                extra = TrKey.RedTeamFlagWinExtra;
                TextRenderer.color = Color.red;
                __instance.BackgroundBar.material.SetColor(ColorID, Color.red);
                break;
            case WinCondition.BlueTeamFlagWin:
                bonus = TrKey.BlueTeamFlagWin;
                extra = TrKey.BlueTeamFlagWinExtra;
                TextRenderer.color = Color.blue;
                __instance.BackgroundBar.material.SetColor(ColorID, Color.blue);
                break;
            case WinCondition.ThiefModePoliceWin:
                bonus = TrKey.ThiefModePoliceWin;
                extra = TrKey.ThiefModePoliceWinExtra;
                TextRenderer.color = PoliceAndThief.PolicePlayerColor;
                __instance.BackgroundBar.material.SetColor(ColorID, PoliceAndThief.PolicePlayerColor);
                break;
            case WinCondition.ThiefModeThiefWin:
                bonus = TrKey.ThiefModeThiefWin;
                extra = TrKey.ThiefModeThiefWinExtra;
                TextRenderer.color = PoliceAndThief.ThiefPlayerColor;
                __instance.BackgroundBar.material.SetColor(ColorID, PoliceAndThief.ThiefPlayerColor);
                break;
            case WinCondition.HotPotatoEnd:
                bonus = TrKey.HotPotatoEnd;
                extra = TrKey.HotPotatoEndExtra;
                TextRenderer.color = Color.cyan;
                __instance.BackgroundBar.material.SetColor(ColorID, Color.cyan);
                break;
            case WinCondition.BattleRoyaleSoloWin:
                bonus = TrKey.BattleRoyaleSoloWin;
                extra = TrKey.BattleRoyaleSoloWinExtra;
                TextRenderer.color = BattleRoyale.IntroColor;
                __instance.BackgroundBar.material.SetColor(ColorID, BattleRoyale.IntroColor);
                break;
            case WinCondition.BattleRoyaleTimeWin:
                bonus = TrKey.BattleRoyaleTimeWin;
                extra = TrKey.BattleRoyaleTimeWinExtra;
                TextRenderer.color = BattleRoyale.IntroColor;
                __instance.BackgroundBar.material.SetColor(ColorID, BattleRoyale.IntroColor);
                break;
            case WinCondition.BattleRoyaleLimeTeamWin:
                bonus = TrKey.BattleRoyaleLimeTeamWin;
                extra = TrKey.BattleRoyaleLimeTeamWinExtra;
                TextRenderer.color = BattleRoyale.LimeTeamColor;
                __instance.BackgroundBar.material.SetColor(ColorID, BattleRoyale.LimeTeamColor);
                break;
            case WinCondition.BattleRoyalePinkTeamWin:
                bonus = TrKey.BattleRoyalePinkTeamWin;
                extra = TrKey.BattleRoyalePinkTeamWinExtra;
                TextRenderer.color = BattleRoyale.PinkTeamColor;
                __instance.BackgroundBar.material.SetColor(ColorID, BattleRoyale.PinkTeamColor);
                break;
            case WinCondition.BattleRoyaleSerialKillerWin:
                bonus = TrKey.BattleRoyaleSerialKillerWin;
                extra = TrKey.BattleRoyaleSerialKillerWinExtra;
                TextRenderer.color = BattleRoyale.SerialKillerColor;
                __instance.BackgroundBar.material.SetColor(ColorID, BattleRoyale.SerialKillerColor);
                break;
            default:
                switch (AdditionalTempData.GameOverReason)
                {
                    case GameOverReason.CrewmatesByVote:
                    case GameOverReason.CrewmatesByTask:
                        bonus = TrKey.CrewmateWin;
                        extra = TrKey.CrewmateWinExtra;
                        TextRenderer.color = Palette.CrewmateBlue;
                        break;
                    case GameOverReason.ImpostorsByVote:
                    case GameOverReason.ImpostorsBySabotage:
                    case GameOverReason.ImpostorsByKill:
                        bonus = TrKey.ImpostorWin;
                        extra = TrKey.ImpostorWinExtra;
                        TextRenderer.color = Palette.ImpostorRed;
                        break;
                }

                break;
        }

        var extraText = new StringBuilder();
        foreach (var w in AdditionalTempData.AdditionalWinConditions)
        {
            switch (w)
            {
                // case EWinCondition.OpportunistWin:
                //     extraText += Tr.Get(TranslateKey.opportunistExtra);
                //     break;
                case WinCondition.LoversTeamWin:
                    extraText.Append(Tr.Get(TrKey.LoversExtra));
                    break;
            }
        }

        TextRenderer.text = extraText.Length > 0 ? string.Format(Tr.Get(extra), extraText) : Tr.Get(bonus);

        if (MapSettings.ShowRoleSummary)
        {
            if (Camera.main != null)
            {
                var position = Camera.main.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = Object.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new(1f, 1f, 1f);

                var roleSummaryText = new StringBuilder();
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
                roleSummaryTextMeshRectTransform.anchoredPosition = new(position.x + 3.5f, position.y - 0.1f);
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
        if (!Jester.TriggerJesterWin) return false;
        UncheckedEndGame(CustomGameOverReason.JesterWin);
        return true;
    }

    public static bool CheckAndEndGameForArsonistWin()
    {
        if (!Arsonist.TriggerArsonistWin) return false;
        UncheckedEndGame(CustomGameOverReason.ArsonistWin);
        return true;
    }

    public static bool CheckAndEndGameForVultureWin()
    {
        if (!Vulture.TriggerVultureWin) return false;
        UncheckedEndGame(CustomGameOverReason.VultureWin);
        return true;
    }

    public static bool CheckAndEndGameForSabotageWin()
    {
        if (MapUtilities.Systems == null) return false;
        var systems = MapUtilities.Systems;
        if (systems.TryGetValue(SystemTypes.LifeSupp, out var systemType) && systemType != null)
        {
            var lifeSuppSystemType = systemType.TryCast<LifeSuppSystemType>();
            if (lifeSuppSystemType is { Countdown: < 0f })
            {
                IsO2Win = true;
                EndGameForO2Sabotage();
                lifeSuppSystemType.Countdown = 10000f;
                return true;
            }
        }

        if ((!systems.TryGetValue(SystemTypes.Reactor, out var reactor) && !systems.TryGetValue(SystemTypes.Laboratory, out reactor)) || reactor == null) return false;
        var criticalSystem = reactor.TryCast<ICriticalSabotage>();
        if (criticalSystem is not { Countdown: < 0f }) return false;
        EndGameForSabotage();
        criticalSystem.ClearSabotage();
        return true;
    }

    public static bool CheckAndEndGameForLoverWin(PlayerStatistics statistics)
    {
        if (statistics.CouplesAlive != 1 || statistics.TotalAlive > 3) return false;
        UncheckedEndGame(CustomGameOverReason.LoversWin);
        return true;
    }

    public static bool CheckAndEndGameForJackalWin(PlayerStatistics statistics)
    {
        if (statistics.TeamJackalAlive < statistics.TotalAlive - statistics.TeamJackalAlive || statistics.TeamImpostorsAlive != 0 || (statistics.TeamJackalLovers != 0 && statistics.TeamJackalLovers < statistics.CouplesAlive * 2)) return false;
        UncheckedEndGame(CustomGameOverReason.TeamJackalWin);
        return true;
    }

    public static bool CheckAndEndGameForImpostorWin(PlayerStatistics statistics)
    {
        if (statistics.TeamImpostorsAlive < statistics.TotalAlive - statistics.TeamImpostorsAlive || statistics.TeamJackalAlive != 0 || (statistics.TeamImpostorLovers != 0 && statistics.TeamImpostorLovers < statistics.CouplesAlive * 2)) return false;
        var endReason = GameData.LastDeathReason switch
        {
            DeathReason.Exile => GameOverReason.ImpostorsByVote,
            DeathReason.Kill => GameOverReason.ImpostorsByKill,
            _ => GameOverReason.ImpostorsByVote,
        };
        UncheckedEndGame(endReason);
        return true;
    }

    public static bool CheckAndEndGameForCrewmateWin(PlayerStatistics statistics)
    {
        if (statistics.TeamCrew <= 0 || statistics.TeamImpostorsAlive != 0 || statistics.TeamJackalAlive != 0) return false;
        UncheckedEndGame(GameOverReason.CrewmatesByVote);
        return true;
    }

    private static void EndGameForSabotage()
    {
        UncheckedEndGame(GameOverReason.ImpostorsBySabotage);
    }

    private static void EndGameForO2Sabotage()
    {
        UncheckedEndGame(CustomGameOverReason.O2SabotageEnd);
    }

    private static void UncheckedEndGame(GameOverReason reason)
    {
        GameManager.Instance.RpcEndGame(reason, false);
        using var sender = new RPCSender(PlayerControl.LocalPlayer.NetId, CustomRPC.UncheckedEndGame);
        sender.Write((byte)reason);
        sender.Write(IsO2Win);
        RPCProcedure.UncheckedEndGame((byte)reason, IsO2Win);
    }

    private static void UncheckedEndGame(CustomGameOverReason reason)
    {
        UncheckedEndGame((GameOverReason)reason);
    }

    public static bool CheckAndEndGameForDrawFlagWin()
    {
        if (!CaptureTheFlag.TriggerDrawWin) return false;
        UncheckedEndGame(CustomGameOverReason.DrawTeamWin);
        return true;
    }

    public static bool CheckAndEndGameForRedTeamFlagWin()
    {
        if (!CaptureTheFlag.TriggerRedTeamWin) return false;
        UncheckedEndGame(CustomGameOverReason.RedTeamFlagWin);
        return true;
    }

    public static bool CheckAndEndGameForBlueTeamFlagWin()
    {
        if (!CaptureTheFlag.TriggerBlueTeamWin) return false;
        UncheckedEndGame(CustomGameOverReason.BlueTeamFlagWin);
        return true;
    }

    public static bool CheckAndEndGameForThiefModeThiefWin()
    {
        if (!PoliceAndThief.TriggerThiefWin) return false;
        UncheckedEndGame(CustomGameOverReason.ThiefModeThiefWin);
        return true;
    }

    public static bool CheckAndEndGameForThiefModePoliceWin()
    {
        if (!PoliceAndThief.TriggerPoliceWin) return false;
        UncheckedEndGame(CustomGameOverReason.ThiefModePoliceWin);
        return true;
    }

    public static bool CheckAndEndGameForHotPotatoEnd()
    {
        if (!HotPotato.TriggerHotPotatoEnd) return false;
        UncheckedEndGame(CustomGameOverReason.HotPotatoEnd);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyaleSoloWin()
    {
        if (!BattleRoyale.TriggerSoloWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyaleSoloWin);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyaleTimeWin()
    {
        if (!BattleRoyale.TriggerTimeWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyaleTimeWin);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyaleDraw()
    {
        if (!BattleRoyale.TriggerDrawWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyaleDraw);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyaleLimeTeamWin()
    {
        if (!BattleRoyale.TriggerLimeTeamWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyaleLimeTeamWin);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyalePinkTeamWin()
    {
        if (!BattleRoyale.TriggerPinkTeamWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyalePinkTeamWin);
        return true;
    }

    public static bool CheckAndEndGameForBattleRoyaleSerialKillerWin()
    {
        if (!BattleRoyale.TriggerSerialKillerWin) return false;
        UncheckedEndGame(CustomGameOverReason.BattleRoyaleSerialKillerWin);
        return true;
    }
}
