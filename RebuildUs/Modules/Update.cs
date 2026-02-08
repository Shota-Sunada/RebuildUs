namespace RebuildUs.Patches;

public static class Update
{
    private static bool IsUpdating = false;
    private static readonly Dictionary<byte, Color> ColorCache = [];

    public static void SetChatBubbleColor(ChatBubble bubble, string playerName)
    {
        if (bubble == null) return;
        var lp = PlayerControl.LocalPlayer;
        if (lp == null || !lp.IsTeamImpostor()) return;

        foreach (var sourcePlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (sourcePlayer.Data != null && sourcePlayer.Data.PlayerName.Equals(playerName))
            {
                if (sourcePlayer.IsRole(RoleType.Spy))
                {
                    bubble.NameText.color = Palette.ImpostorRed;
                }
                else if (sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.GetRole(sourcePlayer)?.WasTeamRed == true)
                {
                    bubble.NameText.color = Palette.ImpostorRed;
                }
                else if (sourcePlayer.IsRole(RoleType.Jackal) && Jackal.GetRole(sourcePlayer)?.WasTeamRed == true)
                {
                    bubble.NameText.color = Palette.ImpostorRed;
                }

                break;
            }
        }
    }

    public static void UpdatePlayerNamesAndColors()
    {
        IsUpdating = true;
        ColorCache.Clear();

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            IsUpdating = false;
            return;
        }

        bool isLocalImpostor = localPlayer.IsTeamImpostor();
        bool isLocalDead = localPlayer.IsDead();
        bool meetingShow = Helpers.ShowMeetingText;

        // 1. Initialize Base Colors
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null) continue;
            Color baseColor = (isLocalImpostor && p.IsTeamImpostor()) ? Palette.ImpostorRed : Color.white;
            SetPlayerNameColor(p, baseColor);
        }

        // 2. Calculate Role/Modifier Colors (populates ColorCache via SetPlayerNameColor)
        SetNameColors();

        setNameTags();

        // 3. Update Player Instances
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player == null || player.cosmetics == null || player.cosmetics.nameText == null) continue;

            // --- Name Calculation ---
            string finalName = player.CurrentOutfit.PlayerName;
            bool hideName = Helpers.HidePlayerName(localPlayer, player);

            if (hideName)
            {
                finalName = "";
            }
            else if (isLocalDead)
            {
                if (string.IsNullOrEmpty(finalName) && Camouflager.CamouflageTimer > 0f)
                {
                    finalName = player.Data.DefaultOutfit.PlayerName;
                }
                else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                {
                    finalName = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
                }
            }

            TagStringBuilder.Clear();
            TagStringBuilder.Append(finalName);

            if (!string.IsNullOrEmpty(finalName))
            {
                // Role Tags
                var r = PlayerRole.GetRole(player);
                if (r != null)
                {
                    if (!string.IsNullOrEmpty(r.NameTag)) TagStringBuilder.Append(r.NameTag);
                    r.OnUpdateNameTags();
                }

                // Modifier Tags
                foreach (var m in PlayerModifier.GetModifiers(player))
                {
                    if (!string.IsNullOrEmpty(m.NameTag)) TagStringBuilder.Append(m.NameTag);
                    m.OnUpdateNameTags();
                }

                // Lovers
                if (Lovers.IsLovers(player) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead)))
                {
                    TagStringBuilder.Append(Lovers.GetIcon(player));
                }
            }

            string resultText = TagStringBuilder.ToString();
            if (player.cosmetics.nameText.text != resultText) player.cosmetics.nameText.text = resultText;

            if (ColorCache.TryGetValue(player.PlayerId, out Color c))
            {
                if (player.cosmetics.nameText.color != c) player.cosmetics.nameText.color = c;
            }
        }

        // 4. Update Meeting HUD
        if (MeetingHud.Instance != null)
        {
            var playersById = Helpers.AllPlayersById();
            foreach (var pva in MeetingHud.Instance.playerStates)
            {
                if (pva == null || pva.NameText == null) continue;
                if (!playersById.TryGetValue(pva.TargetPlayerId, out var target)
                    || target == null
                    || target.Data == null) continue;

                string baseName = target.Data.PlayerName;
                if (isLocalDead)
                {
                    if (string.IsNullOrEmpty(baseName) && Camouflager.CamouflageTimer > 0f)
                    {
                        baseName = target.Data.DefaultOutfit?.PlayerName ?? "";
                    }
                    else if (target.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    {
                        baseName = $"{target.CurrentOutfit?.PlayerName} ({target.Data.DefaultOutfit?.PlayerName})";
                    }
                }

                TagStringBuilder.Clear();
                TagStringBuilder.Append(baseName);

                // Role Tags (Meeting Only)
                var r = PlayerRole.GetRole(target);
                if (r != null && !string.IsNullOrEmpty(r.NameTag)) TagStringBuilder.Append(r.NameTag);

                // Detective / Hacker
                if (MapSettings.ShowLighterDarker && meetingShow)
                {
                    TagStringBuilder.Append(" (")
                                    .Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId)
                                                ? Tr.Get(TrKey.DetectiveLightLabel)
                                                : Tr.Get(TrKey.DetectiveDarkLabel))
                                    .Append(')');
                }

                // Lovers
                if (Lovers.IsLovers(target) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead)))
                {
                    TagStringBuilder.Append(Lovers.GetIcon(target));
                }

                string resultText = TagStringBuilder.ToString();
                if (pva.NameText.text != resultText) pva.NameText.text = resultText;

                if (ColorCache.TryGetValue(target.PlayerId, out Color c))
                {
                    if (pva.NameText.color != c) pva.NameText.color = c;
                }
            }
        }

        IsUpdating = false;
    }

    public static void SetPlayerNameColor(PlayerControl p, Color color)
    {
        if (IsUpdating)
        {
            ColorCache[p.PlayerId] = color;
            return;
        }

        p.cosmetics.nameText.color = color;
        if (MeetingHud.Instance != null)
        {
            foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
            {
                if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                {
                    player.NameText.color = color;
                }
            }
        }
    }

    public static void SetNameColors()
    {
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.Roles:
                var lp = PlayerControl.LocalPlayer;
                if (lp == null) return;

                // 1. Set Local Player Color
                var roleInstance = PlayerRole.GetRole(lp);
                if (roleInstance != null)
                {
                    SetPlayerNameColor(lp, roleInstance.RoleColor);
                }

                foreach (var mod in PlayerModifier.GetModifiers(lp))
                {
                    SetPlayerNameColor(lp, mod.ModifierColor);
                }

                // 2. Process logic-heavy vision (Jackal seeing Sidekick, Spy seeing Impostors, etc.)
                foreach (var r in PlayerRole.AllRoles) r.OnUpdateNameColors();
                foreach (var m in PlayerModifier.AllModifiers) m.OnUpdateNameColors();
                break;

            case CustomGameMode.CaptureTheFlag:
                if (CaptureTheFlag.stealerPlayer != null)
                {
                    SetPlayerNameColor(CaptureTheFlag.stealerPlayer, Palette.PlayerColors[15]);
                }

                foreach (PlayerControl redplayer in CaptureTheFlag.redteamFlag)
                {
                    if (redplayer != null)
                    {
                        SetPlayerNameColor(redplayer, Palette.PlayerColors[0]);
                    }
                }

                foreach (PlayerControl blueplayer in CaptureTheFlag.blueteamFlag)
                {
                    if (blueplayer != null)
                    {
                        SetPlayerNameColor(blueplayer, Palette.PlayerColors[1]);
                    }
                }

                break;

            case CustomGameMode.PoliceAndThieves:
                foreach (PlayerControl policeplayer in PoliceAndThief.policeTeam)
                {
                    if (policeplayer != null)
                    {
                        if (PoliceAndThief.policeplayer02 != null && policeplayer == PoliceAndThief.policeplayer02
                            || PoliceAndThief.policeplayer04 != null && policeplayer == PoliceAndThief.policeplayer04)
                        {
                            SetPlayerNameColor(policeplayer, Palette.PlayerColors[5]);
                        }
                        else
                        {
                            SetPlayerNameColor(policeplayer, Palette.PlayerColors[10]);
                        }
                    }
                }

                foreach (PlayerControl thiefplayer in PoliceAndThief.thiefTeam)
                {
                    if (thiefplayer != null)
                    {
                        SetPlayerNameColor(thiefplayer, Palette.PlayerColors[16]);
                    }
                }

                break;

            case CustomGameMode.HotPotato:
                foreach (PlayerControl notpotatoplayer in HotPotato.notPotatoTeam)
                {
                    if (notpotatoplayer != null)
                    {
                        SetPlayerNameColor(notpotatoplayer, Palette.PlayerColors[10]);
                    }
                }

                foreach (PlayerControl explodedpotatoplayer in HotPotato.explodedPotatoTeam)
                {
                    if (explodedpotatoplayer != null)
                    {
                        SetPlayerNameColor(explodedpotatoplayer, Palette.PlayerColors[9]);
                    }
                }

                if (HotPotato.hotPotatoPlayer != null)
                {
                    SetPlayerNameColor(HotPotato.hotPotatoPlayer, Palette.PlayerColors[15]);
                }

                break;

            case CustomGameMode.BattleRoyale:
                if (BattleRoyale.matchType == 0)
                {
                    foreach (PlayerControl soloPlayer in BattleRoyale.soloPlayerTeam)
                    {
                        if (soloPlayer != null)
                        {
                            SetPlayerNameColor(soloPlayer, Palette.PlayerColors[2]);
                        }
                    }
                }
                else
                {
                    if (BattleRoyale.serialKiller != null)
                    {
                        SetPlayerNameColor(BattleRoyale.serialKiller, Palette.PlayerColors[15]);
                    }

                    foreach (PlayerControl limeplayer in BattleRoyale.limeTeam)
                    {
                        if (limeplayer != null)
                        {
                            SetPlayerNameColor(limeplayer, Palette.PlayerColors[11]);
                        }
                    }

                    foreach (PlayerControl pinkplayer in BattleRoyale.pinkTeam)
                    {
                        if (pinkplayer != null)
                        {
                            SetPlayerNameColor(pinkplayer, Palette.PlayerColors[13]);
                        }
                    }
                }

                break;
        }
    }

    private static readonly StringBuilder TagStringBuilder = new();

    private static void setNameTags()
    {
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.BattleRoyale:
                // BR Lives
                if (BattleRoyale.matchType == 0)
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        if (BattleRoyale.soloPlayer01 != null)
                        {
                            BattleRoyale.soloPlayer01.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer01Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer02 != null)
                        {
                            BattleRoyale.soloPlayer02.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer02Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer03 != null)
                        {
                            BattleRoyale.soloPlayer03.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer03Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer04 != null)
                        {
                            BattleRoyale.soloPlayer04.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer04Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer05 != null)
                        {
                            BattleRoyale.soloPlayer05.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer05Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer06 != null)
                        {
                            BattleRoyale.soloPlayer06.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer06Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer07 != null)
                        {
                            BattleRoyale.soloPlayer07.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer07Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer08 != null)
                        {
                            BattleRoyale.soloPlayer08.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer08Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer09 != null)
                        {
                            BattleRoyale.soloPlayer09.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer09Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer10 != null)
                        {
                            BattleRoyale.soloPlayer10.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer10Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer11 != null)
                        {
                            BattleRoyale.soloPlayer11.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer11Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer12 != null)
                        {
                            BattleRoyale.soloPlayer12.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer12Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer13 != null)
                        {
                            BattleRoyale.soloPlayer13.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer13Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer14 != null)
                        {
                            BattleRoyale.soloPlayer14.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer14Lifes + "♥)");
                        }

                        if (BattleRoyale.soloPlayer15 != null)
                        {
                            BattleRoyale.soloPlayer15.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.soloPlayer15Lifes + "♥)");
                        }
                    }
                    else
                    {
                        if (BattleRoyale.soloPlayer01 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer01)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer01Lifes + "♥)");
                            BattleRoyale.soloPlayer01.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer02 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer02)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer02Lifes + "♥)");
                            BattleRoyale.soloPlayer02.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer03 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer03)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer03Lifes + "♥)");
                            BattleRoyale.soloPlayer03.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer04 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer04)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer04Lifes + "♥)");
                            BattleRoyale.soloPlayer04.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer05 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer05)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer05Lifes + "♥)");
                            BattleRoyale.soloPlayer05.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer06 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer06)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer06Lifes + "♥)");
                            BattleRoyale.soloPlayer06.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer07 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer07)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer07Lifes + "♥)");
                            BattleRoyale.soloPlayer07.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer08 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer08)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer08Lifes + "♥)");
                            BattleRoyale.soloPlayer08.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer09 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer09)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer09Lifes + "♥)");
                            BattleRoyale.soloPlayer09.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer10 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer10)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer10Lifes + "♥)");
                            BattleRoyale.soloPlayer10.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer11 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer11)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer11Lifes + "♥)");
                            BattleRoyale.soloPlayer11.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer12 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer12)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer12Lifes + "♥)");
                            BattleRoyale.soloPlayer12.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer13 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer13)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer13Lifes + "♥)");
                            BattleRoyale.soloPlayer13.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer14 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer14)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer14Lifes + "♥)");
                            BattleRoyale.soloPlayer14.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.soloPlayer15 != null && PlayerControl.LocalPlayer == BattleRoyale.soloPlayer15)
                        {
                            string suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                       " (" + BattleRoyale.soloPlayer15Lifes + "♥)");
                            BattleRoyale.soloPlayer15.cosmetics.nameText.text += suffix;
                        }
                    }
                }
                else
                {
                    foreach (PlayerControl limePlayer in BattleRoyale.limeTeam)
                    {
                        if (limePlayer == PlayerControl.LocalPlayer)
                        {
                            if (BattleRoyale.limePlayer01 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer01Lifes + "♥)");
                                BattleRoyale.limePlayer01.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer02 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer02Lifes + "♥)");
                                BattleRoyale.limePlayer02.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer03 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer03Lifes + "♥)");
                                BattleRoyale.limePlayer03.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer04 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer04Lifes + "♥)");
                                BattleRoyale.limePlayer04.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer05 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer05Lifes + "♥)");
                                BattleRoyale.limePlayer05.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer06 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer06Lifes + "♥)");
                                BattleRoyale.limePlayer06.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.limePlayer07 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                           " (" + BattleRoyale.limePlayer07Lifes + "♥)");
                                BattleRoyale.limePlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }

                    foreach (PlayerControl pinkPlayer in BattleRoyale.pinkTeam)
                    {
                        if (pinkPlayer == PlayerControl.LocalPlayer)
                        {
                            if (BattleRoyale.pinkPlayer01 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer01Lifes + "♥)");
                                BattleRoyale.pinkPlayer01.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer02 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer02Lifes + "♥)");
                                BattleRoyale.pinkPlayer02.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer03 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer03Lifes + "♥)");
                                BattleRoyale.pinkPlayer03.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer04 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer04Lifes + "♥)");
                                BattleRoyale.pinkPlayer04.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer05 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer05Lifes + "♥)");
                                BattleRoyale.pinkPlayer05.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer06 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer06Lifes + "♥)");
                                BattleRoyale.pinkPlayer06.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.pinkPlayer07 != null)
                            {
                                string suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                           " (" + BattleRoyale.pinkPlayer07Lifes + "♥)");
                                BattleRoyale.pinkPlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }

                    if (BattleRoyale.serialKiller != null && PlayerControl.LocalPlayer == BattleRoyale.serialKiller)
                    {
                        string suffix = Helpers.Cs(BattleRoyale.SerialKillerColor,
                                                   " (" + BattleRoyale.serialKillerLifes + "♥)");
                        BattleRoyale.serialKiller.cosmetics.nameText.text += suffix;
                    }
                }

                break;
        }
    }

    public static void UpdateImpostorKillButton(HudManager __instance)
    {
        if (__instance == null || __instance.KillButton == null) return;
        if (PlayerControl.LocalPlayer?.Data?.Role?.IsImpostor != true) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        bool enabled = Helpers.ShowButtons;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Vampire))
        {
            enabled &= false;
        }
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanKill)
        {
            enabled &= false;
        }
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Janitor))
        {
            enabled &= false;
        }

        if (enabled) __instance.KillButton.Show();
        else __instance.KillButton.Hide();
    }

    public static void UpdateUseButton(HudManager __instance)
    {
        if (__instance?.UseButton == null) return;
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    public static void UpdateSabotageButton(HudManager __instance)
    {
        if (__instance?.SabotageButton == null) return;
        if (MeetingHud.Instance) __instance.SabotageButton.Hide();
    }

    public static void UpdateVentButton(HudManager __instance)
    {
        if (__instance?.ImpostorVentButton == null) return;
        if (MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
    }

    public static bool activatedReportButtonAfterCustomMode = false;

    public static void UpdateReportButton(HudManager __instance)
    {
        if (__instance?.ReportButton == null) return;
        if (MeetingHud.Instance)
        {
            __instance.ReportButton.Hide();
        }

        if (MapSettings.GameMode is CustomGameMode.Roles)
        {
            if (!activatedReportButtonAfterCustomMode)
            {
                __instance.ReportButton.Show();
                activatedReportButtonAfterCustomMode = true;
            }

            return;
        }

        bool enabled = true;
        if (MapSettings.GameMode is not CustomGameMode.Roles)
        {
            enabled = false;
        }

        enabled &= __instance.ReportButton.isActiveAndEnabled;

        if (enabled)
        {
            __instance.ReportButton.Show();
        }
        else
        {
            __instance.ReportButton.Hide();
        }
    }

    public static void StopCooldown(PlayerControl __instance)
    {
        if (CustomOptionHolder.StopCooldownOnFixingElecSabotage.GetBool())
        {
            if (Helpers.IsOnElecTask())
            {
                __instance.SetKillTimer(__instance.killTimer + Time.fixedDeltaTime);
            }
        }
    }

    public static void ImpostorSetTarget()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || !localPlayer.Data.Role.IsImpostor || !localPlayer.CanMove || localPlayer.Data.IsDead)
        {
            if (FastDestroyableSingleton<HudManager>.Instance)
            {
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            }

            return;
        }

        bool specialTeamRedExists = false;
        if (Spy.Exists)
        {
            specialTeamRedExists = true;
        }
        else
        {
            foreach (var sk in Sidekick.Players)
            {
                if (sk.WasTeamRed)
                {
                    specialTeamRedExists = true;
                    break;
                }
            }

            if (!specialTeamRedExists)
            {
                foreach (var jk in Jackal.Players)
                {
                    if (jk.WasTeamRed)
                    {
                        specialTeamRedExists = true;
                        break;
                    }
                }
            }
        }

        PlayerControl target;
        if (specialTeamRedExists)
        {
            if (Spy.ImpostorsCanKillAnyone)
            {
                target = Helpers.SetTarget(false, true);
            }
            else
            {
                List<PlayerControl> listP = [.. Spy.AllPlayers];
                foreach (var sidekick in Sidekick.Players)
                {
                    if (sidekick.WasTeamRed)
                    {
                        listP.Add(sidekick.Player);
                    }
                }

                foreach (var jackal in Jackal.Players)
                {
                    if (jackal.WasTeamRed)
                    {
                        listP.Add(jackal.Player);
                    }
                }

                target = Helpers.SetTarget(true, true, listP);
            }
        }
        else
        {
            target = Helpers.SetTarget(true, true);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
    }

    public static void PlayerSizeUpdate(PlayerControl p)
    {
        if (p == null) return;

        CircleCollider2D collider = p.GetComponent<CircleCollider2D>();
        if (collider == null) return;

        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        collider.radius = Mini.DefaultColliderRadius;
        collider.offset = Mini.DefaultColliderOffset * Vector2.down;

        // Set adapted player size to Mini and Morphing
        if (Camouflager.CamouflageTimer > 0f) return;

        Mini miniRole = null;
        if (p.HasModifier(ModifierType.Mini))
        {
            miniRole = Mini.GetModifier(p);
        }
        else if (Morphing.Exists
                 && p.IsRole(RoleType.Morphing)
                 && Morphing.MorphTimer > 0f
                 && Morphing.MorphTarget != null
                 && Morphing.MorphTarget.HasModifier(ModifierType.Mini))
        {
            miniRole = Mini.GetModifier(Morphing.MorphTarget);
        }

        if (miniRole != null)
        {
            float growingProgress = miniRole.GrowingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            float correctedColliderRadius = Mini.DefaultColliderRadius * 0.7f / scale;

            p.transform.localScale = new Vector3(scale, scale, 1f);
            collider.radius = correctedColliderRadius;
        }
    }

    public static void CamouflageAndMorphActions()
    {
        float oldCamouflageTimer = Camouflager.CamouflageTimer;
        float oldMorphTimer = Morphing.MorphTimer;

        Camouflager.CamouflageTimer -= Time.deltaTime;
        Morphing.MorphTimer -= Time.deltaTime;

        // Everyone but morphing reset
        if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f)
        {
            Camouflager.ResetCamouflage();
        }

        // Morphing reset
        if (oldMorphTimer > 0f && Morphing.MorphTimer <= 0f)
        {
            Morphing.ResetMorph();
        }
    }

    public static void TimerUpdate()
    {
        var deltaTime = Time.deltaTime;

        switch (MapSettings.GameMode)
        {
            case CustomGameMode.CaptureTheFlag:
                // CTF timers
                RebuildUs.progressStart += deltaTime;
                MapSettings.gamemodeMatchDuration -= deltaTime;
                if (RebuildUs.progress != null)
                {
                    RebuildUs.progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.gamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.progressStart / RebuildUs.progressEnd);
                }

                if (MapSettings.gamemodeMatchDuration <= 0)
                {
                    // both teams with same points = Draw
                    if (CaptureTheFlag.currentRedTeamPoints == CaptureTheFlag.currentBlueTeamPoints)
                    {
                        CaptureTheFlag.triggerDrawWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.DrawTeamWin, false);
                    }
                    // Red team more points than blue team = red team win
                    else if (CaptureTheFlag.currentRedTeamPoints > CaptureTheFlag.currentBlueTeamPoints)
                    {
                        CaptureTheFlag.triggerRedTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RedTeamFlagWin, false);
                    }
                    // otherwise blue team win
                    else
                    {
                        CaptureTheFlag.triggerBlueTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BlueTeamFlagWin, false);
                    }
                }

                break;
            case CustomGameMode.PoliceAndThieves:
                // PT timers
                PoliceAndThief.policeplayer01lightTimer -= deltaTime;
                PoliceAndThief.policeplayer02lightTimer -= deltaTime;
                PoliceAndThief.policeplayer03lightTimer -= deltaTime;
                PoliceAndThief.policeplayer04lightTimer -= deltaTime;
                PoliceAndThief.policeplayer05lightTimer -= deltaTime;
                PoliceAndThief.policeplayer06lightTimer -= deltaTime;

                RebuildUs.progressStart += deltaTime;
                MapSettings.gamemodeMatchDuration -= deltaTime;
                if (RebuildUs.progress != null)
                {
                    RebuildUs.progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.gamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.progressStart / RebuildUs.progressEnd);
                }

                if (MapSettings.gamemodeMatchDuration <= 0)
                {
                    PoliceAndThief.triggerPoliceWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                }

                break;
            case CustomGameMode.HotPotato:
                // HP timers
                if (HotPotato.firstPotatoTransfered)
                {
                    HotPotato.timeforTransfer -= deltaTime;

                    if (HotPotato.timeforTransfer <= 0
                        && !HotPotato.hotPotatoPlayer.Data.IsDead
                        && AmongUsClient.Instance.AmHost)
                    {
                        // Ensure host send an RPC so the time doesn't bug
                        MessageWriter winWriter =
                            AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                                                       (byte)CustomRPC.HotPotatoExploded,
                                                                       Hazel.SendOption.Reliable, -1);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.hotPotatoExploded();
                    }

                    RebuildUs.progressStart += deltaTime;
                    MapSettings.gamemodeMatchDuration -= deltaTime;
                    if (RebuildUs.progress != null)
                    {
                        RebuildUs.progress.GetComponentInChildren<TextMeshPro>().text =
                            new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                                  .Append("</color>")
                                                                  .Append(MapSettings.gamemodeMatchDuration
                                                                              .ToString("F0"))
                                                                  .Append(" | <color=#FF8000FF>")
                                                                  .Append(Tr.Get(TrKey.HotPotatoStatus))
                                                                  .Append("</color>")
                                                                  .Append(HotPotato.timeforTransfer.ToString("F0"))
                                                                  .ToString();
                        RebuildUs.progress.GetComponent<ProgressTracker>().curValue =
                            Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                       RebuildUs.progressStart / RebuildUs.progressEnd);
                    }

                    if (MapSettings.gamemodeMatchDuration <= 0)
                    {
                        HotPotato.triggerHotPotatoEnd = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                    }
                }

                break;
            case CustomGameMode.BattleRoyale:
                // BR timers
                RebuildUs.progressStart += deltaTime;
                MapSettings.gamemodeMatchDuration -= deltaTime;
                if (RebuildUs.progress != null)
                {
                    RebuildUs.progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.gamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.progressStart / RebuildUs.progressEnd);
                }

                if (MapSettings.gamemodeMatchDuration <= 0)
                {
                    if (BattleRoyale.matchType == 2)
                    {
                        if (BattleRoyale.serialKiller != null)
                        {
                            // all teams with same points = Draw
                            if (BattleRoyale.limePoints == BattleRoyale.pinkPoints
                                && BattleRoyale.pinkPoints == BattleRoyale.serialKillerPoints)
                            {
                                BattleRoyale.triggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                            // Lime team more points than pink team and serial killer = lime team win
                            else if (BattleRoyale.limePoints > BattleRoyale.pinkPoints
                                     && BattleRoyale.limePoints > BattleRoyale.serialKillerPoints)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleLimeTeamWin, false);
                            }
                            // otherwise pink team win
                            else if (BattleRoyale.pinkPoints > BattleRoyale.limePoints
                                     && BattleRoyale.pinkPoints > BattleRoyale.serialKillerPoints)
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyalePinkTeamWin, false);
                            }
                            // otherwise serial killer win
                            else if (BattleRoyale.serialKillerPoints > BattleRoyale.limePoints
                                     && BattleRoyale.serialKillerPoints > BattleRoyale.pinkPoints)
                            {
                                BattleRoyale.triggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleSerialKillerWin, false);
                            }
                            // draw between some of the teams
                            else
                            {
                                BattleRoyale.triggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                        }
                        else
                        {
                            // both teams with same points = Draw
                            if (BattleRoyale.limePoints == BattleRoyale.pinkPoints)
                            {
                                BattleRoyale.triggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                            // Lime team more points than pink team = lime team win
                            else if (BattleRoyale.limePoints > BattleRoyale.pinkPoints)
                            {
                                BattleRoyale.triggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleLimeTeamWin, false);
                            }
                            // otherwise pink team win
                            else
                            {
                                BattleRoyale.triggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        BattleRoyale.triggerTimeWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleTimeWin,
                                                        false);
                    }
                }

                break;
        }
    }

    public static void UpdateMiniMap()
    {
        if (MapBehaviour.Instance != null && MapBehaviour.Instance.IsOpen)
        {
            if (MapSettings.GameMode is not CustomGameMode.Roles)
            {
                switch (GameOptionsManager.Instance.currentGameOptions.MapId)
                {
                    case 0:
                        GameObject minimapSabotageSkeld = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                        minimapSabotageSkeld.SetActive(false);
                        if (RebuildUs.activatedSensei && !RebuildUs.updatedSenseiMinimap)
                        {
                            GameObject mymap = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/Background");
                            mymap.GetComponent<SpriteRenderer>().sprite = AssetLoader.customMinimap.GetComponent<SpriteRenderer>().sprite;
                            GameObject hereindicator = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/HereIndicatorParent");
                            hereindicator.transform.position = hereindicator.transform.position + new Vector3(0.23f, -0.8f, 0);

                            // Map room names
                            GameObject minimapNames = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/RoomNames (1)");
                            minimapNames.transform.GetChild(0).transform.position =
                                minimapNames.transform.GetChild(0).transform.position
                                + new Vector3(0f, -0.5f, 0); // Upper engine
                            minimapNames.transform.GetChild(2).transform.position =
                                minimapNames.transform.GetChild(2).transform.position
                                + new Vector3(0.7f, -0.55f, 0); // Reactor
                            minimapNames.transform.GetChild(3).transform.position =
                                minimapNames.transform.GetChild(3).transform.position
                                + new Vector3(1.75f, 2.37f, 0); // security
                            minimapNames.transform.GetChild(4).transform.position =
                                minimapNames.transform.GetChild(4).transform.position
                                + new Vector3(0.89f, -1.18f, 0); // medbey
                            minimapNames.transform.GetChild(5).transform.position =
                                minimapNames.transform.GetChild(5).transform.position
                                + new Vector3(0.52f, -1.32f, 0); // Cafetería
                            minimapNames.transform.GetChild(6).transform.position =
                                minimapNames.transform.GetChild(6).transform.position
                                + new Vector3(1f, -1.59f, 0); // weapons
                            minimapNames.transform.GetChild(7).transform.position =
                                minimapNames.transform.GetChild(7).transform.position
                                + new Vector3(-1.72f, -3.03f, 0); // nav
                            minimapNames.transform.GetChild(8).transform.position =
                                minimapNames.transform.GetChild(8).transform.position
                                + new Vector3(-0.08f, 1.45f, 0); // shields
                            minimapNames.transform.GetChild(9).transform.position =
                                minimapNames.transform.GetChild(9).transform.position
                                + new Vector3(1.1f, 2.88f, 0); // cooms
                            minimapNames.transform.GetChild(10).transform.position =
                                minimapNames.transform.GetChild(10).transform.position
                                + new Vector3(-2.2f, -0.82f, 0); // storage
                            minimapNames.transform.GetChild(11).transform.position =
                                minimapNames.transform.GetChild(11).transform.position
                                + new Vector3(0.32f, -1.02f, 0); // Admin
                            minimapNames.transform.GetChild(12).transform.position =
                                minimapNames.transform.GetChild(12).transform.position
                                + new Vector3(0.53f, -2.1f, 0); // electrical
                            minimapNames.transform.GetChild(13).transform.position =
                                minimapNames.transform.GetChild(13).transform.position
                                + new Vector3(-3.5f, -0.5f, 0); // o2

                            // Map sabotage
                            GameObject minimapSabotage =
                                GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                            minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // cafeteria doors
                            minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // medbey doors
                            minimapSabotage.transform
                                           .GetChild(3)
                                           .transform
                                           .GetChild(0)
                                           .gameObject
                                           .SetActive(false); // electrical doors
                            minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // upper engine doors
                            minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // lower engine doors
                            minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // storage doors
                            minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // security doors

                            minimapSabotage.transform.GetChild(1).transform.position =
                                minimapSabotage.transform.GetChild(1).transform.position
                                + new Vector3(0.95f, 3.3f, 0); // Sabotage cooms
                            minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position =
                                minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position
                                + new Vector3(0.165f, -1.2f, 0); // Sabotage electrical
                            minimapSabotage.transform.GetChild(4).transform.position =
                                minimapSabotage.transform.GetChild(4).transform.position
                                + new Vector3(-3f, 0.05f, 0); // Sabotage o2
                            minimapSabotage.transform.GetChild(8).transform.position =
                                minimapSabotage.transform.GetChild(8).transform.position
                                + new Vector3(0.6f, 0.1f, 0); // Sabotage reactor


                            RebuildUs.updatedSenseiMinimap = true;
                        }

                        break;
                    case 1:
                        GameObject minimapSabotageMira =
                            GameObject.Find("Main Camera/Hud/HqMap(Clone)/InfectedOverlay");
                        minimapSabotageMira.SetActive(false);
                        break;
                    case 2:
                        GameObject minimapSabotagePolus =
                            GameObject.Find("Main Camera/Hud/PbMap(Clone)/InfectedOverlay");
                        minimapSabotagePolus.SetActive(false);
                        break;
                    case 3:
                        GameObject minimapSabotageDleks =
                            GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                        minimapSabotageDleks.SetActive(false);
                        break;
                    case 4:
                        GameObject minimapSabotageAirship =
                            GameObject.Find("Main Camera/Hud/AirshipMap(Clone)/InfectedOverlay");
                        minimapSabotageAirship.SetActive(false);
                        break;
                    case 5:
                        GameObject minimapSabotageFungle =
                            GameObject.Find("Main Camera/Hud/FungleMap(Clone)/InfectedOverlay");
                        minimapSabotageFungle.SetActive(false);
                        break;
                    case 6:
                        GameObject minimapSabotageSubmerged =
                            GameObject.Find("Main Camera/Hud/HudMapPrefab(Clone)(Clone)/MapHud/InfectedOverlay");
                        minimapSabotageSubmerged.SetActive(false);
                        break;
                }
            }
            else if (GameOptionsManager.Instance.currentGameOptions.MapId == 0
                     && RebuildUs.activatedSensei
                     && !RebuildUs.updatedSenseiMinimap
                     && MapSettings.GameMode is CustomGameMode.Roles)
            {
                GameObject mymap = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/Background");
                mymap.GetComponent<SpriteRenderer>().sprite = AssetLoader.customMinimap
                                                                        .GetComponent<SpriteRenderer>()
                                                                        .sprite;
                GameObject hereindicator = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/HereIndicatorParent");
                hereindicator.transform.position = hereindicator.transform.position + new Vector3(0.23f, -0.8f, 0);

                // Map room names
                GameObject minimapNames = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/RoomNames (1)");
                minimapNames.transform.GetChild(0).transform.position =
                    minimapNames.transform.GetChild(0).transform.position + new Vector3(0f, -0.5f, 0); // upper engine
                minimapNames.transform.GetChild(2).transform.position =
                    minimapNames.transform.GetChild(2).transform.position + new Vector3(0.7f, -0.55f, 0); // Reactor
                minimapNames.transform.GetChild(3).transform.position =
                    minimapNames.transform.GetChild(3).transform.position + new Vector3(1.75f, 2.37f, 0); // security
                minimapNames.transform.GetChild(4).transform.position =
                    minimapNames.transform.GetChild(4).transform.position + new Vector3(0.89f, -1.18f, 0); // medbey
                minimapNames.transform.GetChild(5).transform.position =
                    minimapNames.transform.GetChild(5).transform.position + new Vector3(0.52f, -1.32f, 0); // Cafetería
                minimapNames.transform.GetChild(6).transform.position =
                    minimapNames.transform.GetChild(6).transform.position + new Vector3(1f, -1.59f, 0); // weapons
                minimapNames.transform.GetChild(7).transform.position =
                    minimapNames.transform.GetChild(7).transform.position + new Vector3(-1.72f, -3.03f, 0); // nav
                minimapNames.transform.GetChild(8).transform.position =
                    minimapNames.transform.GetChild(8).transform.position + new Vector3(-0.08f, 1.45f, 0); // shields
                minimapNames.transform.GetChild(9).transform.position =
                    minimapNames.transform.GetChild(9).transform.position + new Vector3(1.1f, 2.88f, 0); // cooms
                minimapNames.transform.GetChild(10).transform.position =
                    minimapNames.transform.GetChild(10).transform.position + new Vector3(-2.2f, -0.82f, 0); // storage
                minimapNames.transform.GetChild(11).transform.position =
                    minimapNames.transform.GetChild(11).transform.position + new Vector3(0.32f, -1.02f, 0); // Admin
                minimapNames.transform.GetChild(12).transform.position =
                    minimapNames.transform.GetChild(12).transform.position + new Vector3(0.53f, -2.1f, 0); // elec
                minimapNames.transform.GetChild(13).transform.position =
                    minimapNames.transform.GetChild(13).transform.position + new Vector3(-3.5f, -0.5f, 0); // o2

                // Map sabotage
                GameObject minimapSabotage = GameObject.Find("Main Camera/Hud/ShipMap(Clone)/InfectedOverlay");
                minimapSabotage.transform.GetChild(0).gameObject.SetActive(false); // cafeteria doors
                minimapSabotage.transform.GetChild(2).gameObject.SetActive(false); // medbey doors
                minimapSabotage.transform
                               .GetChild(3)
                               .transform
                               .GetChild(0)
                               .gameObject
                               .SetActive(false); // Puertas electricidad
                minimapSabotage.transform.GetChild(5).gameObject.SetActive(false); // upper engine doors
                minimapSabotage.transform.GetChild(6).gameObject.SetActive(false); // lower engine doors
                minimapSabotage.transform.GetChild(7).gameObject.SetActive(false); // storage doors
                minimapSabotage.transform.GetChild(9).gameObject.SetActive(false); // security doors

                minimapSabotage.transform.GetChild(1).transform.position =
                    minimapSabotage.transform.GetChild(1).transform.position
                    + new Vector3(0.95f, 3.3f, 0); // Sabotage cooms
                minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position =
                    minimapSabotage.transform.GetChild(3).transform.GetChild(1).transform.position
                    + new Vector3(0.165f, -1.2f, 0); // Sabotage elec
                minimapSabotage.transform.GetChild(4).transform.position =
                    minimapSabotage.transform.GetChild(4).transform.position
                    + new Vector3(-3f, 0.05f, 0); // Sabotage o2
                minimapSabotage.transform.GetChild(8).transform.position =
                    minimapSabotage.transform.GetChild(8).transform.position
                    + new Vector3(0.6f, 0.1f, 0); // Sabotage reactor


                RebuildUs.updatedSenseiMinimap = true;
            }

            // If bomb, lights actives or special 1vs1 condition, prevent sabotage open map
            if (GameOptionsManager.Instance.currentGameMode == GameModes.Normal
                && MapSettings.GameMode is CustomGameMode.Roles
                && PlayerControl.LocalPlayer.Data.Role.IsImpostor
                && MapBehaviour.Instance != null
                && MapBehaviour.Instance.IsOpen)
            {
                MapBehaviour.Instance.Close();
            }
        }
    }
}