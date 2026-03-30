using Assets.CoreScripts;

namespace RebuildUs.Core.EndGame;

internal static partial class EndGameMain
{
    internal static void Override(EndGameManager __instance)
    {
        DataManager.Player.Stats.IncrementStat(StatID.GamesFinished);
        __instance.Navigation.HideButtons();
        var flag = GameManager.Instance.DidHumansWin(EndGameResult.CachedGameOverReason);
        if (EndGameResult.CachedGameOverReason == GameOverReason.ImpostorDisconnect)
        {
            DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Draws);
            __instance.WinText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.ImpostorDisconnected);
        }
        else
        {
            var result = false;
            foreach (var cpd in EndGameResult.CachedWinners.GetFastEnumerator())
            {
                if (cpd.IsYou)
                {
                    result = true;
                    break;
                }
            }

            if (result)
            {
                DataManager.Player.Stats.IncrementWinStats(EndGameResult.CachedGameOverReason, (MapNames)GameManager.Instance.LogicOptions.MapId, EndGameResult.CachedLocalPlayer.RoleWhenAlive);
                FastDestroyableSingleton<AchievementManager>.Instance.SetWinMap(GameManager.Instance.LogicOptions.MapId);
                __instance.WinText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Victory);
                __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);

                CachedPlayerData cachedPlayerData = null;
                foreach (var cpd in EndGameResult.CachedWinners.GetFastEnumerator())
                {
                    if (cpd.IsYou)
                    {
                        cachedPlayerData = cpd;
                        break;
                    }
                }

                if (cachedPlayerData != null)
                {
                    FastDestroyableSingleton<UnityTelemetry>.Instance.WonGame(cachedPlayerData.ColorId, cachedPlayerData.HatId, cachedPlayerData.SkinId, cachedPlayerData.PetId, cachedPlayerData.VisorId, cachedPlayerData.NamePlateId);
                }
            }
            else
            {
                DataManager.Player.Stats.IncrementGameResultStat(EndGameResult.CachedGameOverReason, GameResultStat.Losses);
                __instance.WinText.text = FastDestroyableSingleton<TranslationController>.Instance.GetString(StringNames.Defeat);
                __instance.WinText.color = Color.red;
            }
        }

        if (EndGameResult.CachedGameOverReason == GameOverReason.ImpostorDisconnect || AdditionalTempData.GameOverReason == (GameOverReason)CustomGameOverReason.ForceEnd)
        {
            SoundManager.Instance.PlaySound(__instance.DisconnectStinger, false);
        }
        else if (flag)
        {
            SoundManager.Instance.PlayDynamicSound("Stinger", __instance.CrewStinger, false, (DynamicSound.GetDynamicsFunction)__instance.GetStingerVol, SoundManager.Instance.MusicChannel);
        }
        else
        {
            SoundManager.Instance.PlayDynamicSound("Stinger", __instance.ImpostorStinger, false, (DynamicSound.GetDynamicsFunction)__instance.GetStingerVol, SoundManager.Instance.MusicChannel);
        }

        var stops = Mathf.CeilToInt(7.5f);
        var list = new Il2CppSystem.Collections.Generic.List<CachedPlayerData>();
        foreach (var cpd in EndGameResult.CachedWinners.GetFastEnumerator())
        {
            if (cpd.IsYou)
            {
                list.Insert(0, cpd);
            }
            else
            {
                list.Add(cpd);
            }
        }

        var playerRoles = AdditionalTempData.PlayerRoles;
        foreach (var data in playerRoles)
        {
            if (data.Value.Status == FinalStatus.Disconnected)
            {
                var found = false;
                foreach (var cpd in list)
                {
                    if (cpd.PlayerName == data.Value.PlayerName)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    var cpd = EndGameResult.CachedLocalPlayer.PlayerName == data.Value.PlayerName ? EndGameResult.CachedLocalPlayer : null;
                    if (cpd == null)
                    {
                        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        {
                            if (p.Data.PlayerName == data.Value.PlayerName)
                            {
                                cpd = new CachedPlayerData(p.Data);
                                break;
                            }
                        }
                    }

                    if (cpd != null)
                    {
                        list.Add(cpd);
                    }
                }
            }
        }

        Dictionary<string, PlayerRoleInfo> playerRolesDict = [];
        foreach (var pr in playerRoles)
        {
            if (pr.Value != null)
            {
                playerRolesDict[pr.Value.PlayerName] = pr.Value;
            }
        }

        for (var index = 0; index < list.Count; ++index)
        {
            var cachedPlayerData = list[index];
            var num1 = index % 2 == 0 ? -1 : 1;
            var i = (index + 1) / 2;
            var t = i / (float)stops;
            var num2 = Mathf.Lerp(1f, 0.75f, t);
            var num3 = index == 0 ? -8f : -1f;
            var poolablePlayer = UnityObject.Instantiate(__instance.PlayerPrefab, __instance.transform);
            poolablePlayer.transform.localPosition = new Vector3(1f * num1 * i * num2, FloatRange.SpreadToEdges(-1.125f, 0.0f, i, stops), num3 + i * 0.01f) * 0.9f;
            var num4 = Mathf.Lerp(1f, 0.65f, t) * 0.9f;
            var a = new Vector3(num4, num4, 1f);
            poolablePlayer.transform.localScale = a;
            if (cachedPlayerData.IsDead)
            {
                poolablePlayer.SetBodyAsGhost();
                poolablePlayer.SetDeadFlipX(index % 2 == 0);
            }
            else
            {
                poolablePlayer.SetFlipX(index % 2 == 0);
            }
            poolablePlayer.UpdateFromPlayerOutfit(cachedPlayerData.Outfit, PlayerMaterial.MaskType.None, cachedPlayerData.IsDead, true);

            poolablePlayer.cosmetics.nameText.color = UnityEngine.Color.white;
            poolablePlayer.cosmetics.nameText.lineSpacing *= 0.7f;
            poolablePlayer.cosmetics.nameText.transform.localScale = new(1f / a.x, 1f / a.y, 1f / a.z);
            poolablePlayer.cosmetics.nameText.transform.localPosition = new(poolablePlayer.cosmetics.nameText.transform.localPosition.x, poolablePlayer.cosmetics.nameText.transform.localPosition.y - 0.7f, -15f);

            if (playerRolesDict.TryGetValue(cachedPlayerData.PlayerName, out var data))
            {
                poolablePlayer.cosmetics.nameText.text = string.Format("{0}{1}\n<size=80%>{2}</size>", cachedPlayerData.PlayerName, data.NameSuffix, data.RoleNames);
            }
            else
            {
                poolablePlayer.cosmetics.nameText.text = cachedPlayerData.PlayerName;
            }

            if (AprilFoolsMode.ShouldHorseAround() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                poolablePlayer.SetBodyType(PlayerBodyTypes.Normal);
                poolablePlayer.SetFlipX(false);
            }
        }
    }

    internal static void Postfix(EndGameManager __instance)
    {
        // Additional code
        var bonusTextObject = UnityObject.Instantiate(__instance.WinText.gameObject);
        bonusTextObject.transform.position = new(__instance.WinText.transform.position.x, __instance.WinText.transform.position.y - 0.8f, __instance.WinText.transform.position.z);
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
                TextRenderer.color = Jester.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Jester.Color);
                break;
            case WinCondition.ArsonistWin:
                bonusText = "ArsonistWin";
                TextRenderer.color = Arsonist.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Arsonist.Color);
                break;
            case WinCondition.VultureWin:
                bonusText = "VultureWin";
                TextRenderer.color = Vulture.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Vulture.Color);
                break;
            case WinCondition.JackalWin:
                bonusText = "JackalWin";
                TextRenderer.color = Jackal.Color;
                __instance.BackgroundBar.material.SetColor("_Color", Jackal.Color);
                break;
            case WinCondition.MiniLose:
                bonusText = "MiniDied";
                TextRenderer.color = Mini.Color;
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
            case WinCondition.BattleRoyaleLastOneStanding:
                bonusText = "BattleRoyaleLastOneStanding";
                TextRenderer.color = BattleRoyaleMode.BattleRoyaleColor;
                __instance.BackgroundBar.material.SetColor("_Color", BattleRoyaleMode.BattleRoyaleColor);
                break;
            case WinCondition.BattleRoyaleTimeUp:
                bonusText = "BattleRoyaleTimeUp";
                TextRenderer.color = Palette.DisabledGrey;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.DisabledGrey);
                break;
            case WinCondition.Default:
            default:
                switch (AdditionalTempData.GameOverReason)
                {
                    case GameOverReason.CrewmatesByTask or GameOverReason.CrewmatesByVote or GameOverReason.CrewmateDisconnect:
                        bonusText = "CrewmateWin";
                        TextRenderer.color = Palette.CrewmateBlue;
                        break;
                    case GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage or GameOverReason.ImpostorsByVote or GameOverReason.ImpostorDisconnect:
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

        var winnerText = extraText.Length > 0 ? string.Format(Tr.GetDynamic(string.Format("{0}Extra", bonusText)), extraText) : Tr.GetDynamic(bonusText);
        TextRenderer.text = winnerText;

        if (MapSettings.ShowRoleSummary)
        {
            if (Camera.main != null)
            {
                var position = Camera.main.ViewportToWorldPoint(new(0f, 1f, Camera.main.nearClipPlane));
                var roleSummary = UnityObject.Instantiate(__instance.WinText.gameObject);
                roleSummary.transform.position = new(__instance.Navigation.ExitButton.transform.position.x + 0.1f, position.y - 0.1f, -14f);
                roleSummary.transform.localScale = new(1f, 1f, 1f);

                StringBuilder roleSummaryText = new();
                roleSummaryText.AppendLine(string.Format(Tr.Get(TrKey.GameIsOverBecause), winnerText));
                roleSummaryText.AppendLine("<size=50%> </size>");
                var tempL = new List<PlayerRoleInfo>();
                foreach (var tmp in AdditionalTempData.PlayerRoles.Values)
                {
                    tempL.Add(tmp);
                }
                tempL.Sort((x, y) =>
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
                Logger.LogInfo("[Result] {0}", TextRenderer.text);
                Logger.LogInfo("[Result] ---------- Game Result -----------");

                var lines = new Dictionary<byte, string>();
                var winners = new HashSet<string>(StringComparer.Ordinal);
                foreach (var winner in EndGameResult.CachedWinners.GetFastEnumerator())
                {
                    winners.Add(winner.PlayerName);
                }

                switch (GameModeManager.CurrentGameMode)
                {
                    default:
                    case CustomGamemode.Normal:
                        foreach (var (key, value) in PlayerStore.AllPlayerDataOnStarted)
                        {
                            if (AdditionalTempData.PlayerRoles.TryGetValue(key, out var data))
                            {
                                if (data.PlayerName == "")
                                {
                                    continue;
                                }
                                var taskInfo = TaskDisplayManager.GetTaskInfoText(key);
                                var status = Tr.GetDynamic(Enum.GetName(data.Status));
                                var star = winners.Contains(data.PlayerName) && AdditionalTempData.GameOverReason != (GameOverReason)CustomGameOverReason.ForceEnd ? "★" : "";
                                var result = string.Format("{0}<pos=2.5%>{1}{2}<pos=24%>{3}<pos=32%>{4}<pos=40%>{5}", star, data.PlayerName, data.NameSuffix, taskInfo, status, data.RoleNames);
                                lines[key] = result;
                                Logger.LogInfo("[Result] {0}", result);
                            }
                            else
                            {
                                var status = Tr.Get(TrKey.Disconnected);
                                var result = string.Format("<pos=2.5%>{0}<pos=32%>{1}<pos=40%>{2}", value.Name, status, value.Roles);
                                lines[key] = result;
                                Logger.LogInfo("[Result] {0}", result);
                            }
                        }
                        roleSummaryText.AppendLine(string.Format("<pos=2.5%>{0}<pos=24%>{1}<pos=32%>{2}<pos=40%>{3}", Tr.Get(TrKey.ResultName), Tr.Get(TrKey.ResultTask), Tr.Get(TrKey.ResultStatus), Tr.Get(TrKey.ResultRoles)));
                        break;
                    case CustomGamemode.BattleRoyale:
                        foreach (var (key, value) in PlayerStore.AllPlayerDataOnStarted)
                        {
                            if (AdditionalTempData.PlayerRoles.TryGetValue(key, out var data))
                            {
                                if (data.PlayerName == "")
                                {
                                    continue;
                                }
                                var status = Tr.GetDynamic(Enum.GetName(data.Status));
                                var star = winners.Contains(data.PlayerName) && AdditionalTempData.GameOverReason != (GameOverReason)CustomGameOverReason.ForceEnd ? "★" : "";
                                var result = string.Format("{0}<pos=2.5%>{1}{2}<pos=24%>{3}", star, data.PlayerName, data.NameSuffix, status);
                                lines[key] = result;
                                Logger.LogInfo("[Result] {0}", result);
                            }
                            else
                            {
                                var status = Tr.Get(TrKey.Disconnected);
                                var result = string.Format("<pos=2.5%>{0}<pos=24%>{1}", value.Name, status);
                                lines[key] = result;
                                Logger.LogInfo("[Result] {0}", result);
                            }
                        }
                        roleSummaryText.AppendLine(string.Format("<pos=2.5%>{0}<pos=24%>{1}", Tr.Get(TrKey.ResultName), Tr.Get(TrKey.ResultStatus)));
                        break;
                }

                Logger.LogInfo("[Result] --------------------------------");

                var hostId = AmongUsClient.Instance.HostId;
                var sortedKeys = new List<byte>(lines.Count);
                foreach (var key in lines.Keys)
                {
                    sortedKeys.Add(key);
                }

                var keysArray = sortedKeys.ToArray();
                Array.Sort(keysArray, (x, y) =>
                {
                    if (x == hostId) return -1;
                    if (y == hostId) return 1;
                    return x.CompareTo(y);
                });

                foreach (var key in keysArray)
                {
                    roleSummaryText.AppendLine(lines[key]);
                }

                roleSummaryText.AppendLine("<size=50%> </size>");
                roleSummaryText.Append(Tr.Get(TrKey.GameTime));
                roleSummaryText.AppendLine((DateTime.Now - GameHistory.TimeStarted).ToString(@"hh\:mm\:ss"));

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
        }

        AdditionalTempData.Clear();
        RPCProcedure.ResetVariables();
    }
}