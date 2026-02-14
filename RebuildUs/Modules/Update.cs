namespace RebuildUs.Patches;

public static class Update
{
    private static bool _isUpdating;
    private static readonly Dictionary<byte, Color> COLOR_CACHE = [];

    private static readonly StringBuilder TAG_STRING_BUILDER = new();

    public static bool ActivatedReportButtonAfterCustomMode;

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
                    bubble.NameText.color = Palette.ImpostorRed;
                else if (sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.GetRole(sourcePlayer)?.WasTeamRed == true)
                    bubble.NameText.color = Palette.ImpostorRed;
                else if (sourcePlayer.IsRole(RoleType.Jackal) && Jackal.GetRole(sourcePlayer)?.WasTeamRed == true)
                    bubble.NameText.color = Palette.ImpostorRed;

                break;
            }
        }
    }

    public static void UpdatePlayerNamesAndColors()
    {
        _isUpdating = true;
        COLOR_CACHE.Clear();

        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            _isUpdating = false;
            return;
        }

        var isLocalImpostor = localPlayer.IsTeamImpostor();
        var isLocalDead = localPlayer.IsDead();
        var meetingShow = Helpers.ShowMeetingText;

        // 1. Initialize Base Colors
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null) continue;
            var baseColor = isLocalImpostor && p.IsTeamImpostor() ? Palette.ImpostorRed : Color.white;
            SetPlayerNameColor(p, baseColor);
        }

        // 2. Calculate Role/Modifier Colors (populates ColorCache via SetPlayerNameColor)
        SetNameColors();

        SetNameTags();

        // 3. Update Player Instances
        foreach (var player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player == null || player.cosmetics == null || player.cosmetics.nameText == null) continue;

            // --- Name Calculation ---
            var finalName = player.CurrentOutfit.PlayerName;
            var hideName = Helpers.HidePlayerName(localPlayer, player);

            if (hideName)
                finalName = "";
            else if (isLocalDead)
            {
                if (string.IsNullOrEmpty(finalName) && Camouflager.CamouflageTimer > 0f)
                    finalName = player.Data.DefaultOutfit.PlayerName;
                else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                    finalName = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
            }

            TAG_STRING_BUILDER.Clear();
            TAG_STRING_BUILDER.Append(finalName);

            if (!string.IsNullOrEmpty(finalName))
            {
                // Role Tags
                var r = PlayerRole.GetRole(player);
                if (r != null)
                {
                    if (!string.IsNullOrEmpty(r.NameTag)) TAG_STRING_BUILDER.Append(r.NameTag);
                    r.OnUpdateNameTags();
                }

                // Modifier Tags
                foreach (var m in PlayerModifier.GetModifiers(player))
                {
                    if (!string.IsNullOrEmpty(m.NameTag)) TAG_STRING_BUILDER.Append(m.NameTag);
                    m.OnUpdateNameTags();
                }

                // Lovers
                if (Lovers.IsLovers(player) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead)))
                    TAG_STRING_BUILDER.Append(Lovers.GetIcon(player));
            }

            var resultText = TAG_STRING_BUILDER.ToString();
            if (player.cosmetics.nameText.text != resultText) player.cosmetics.nameText.text = resultText;

            if (COLOR_CACHE.TryGetValue(player.PlayerId, out var c))
            {
                if (player.cosmetics.nameText.color != c)
                    player.cosmetics.nameText.color = c;
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

                var baseName = target.Data.PlayerName;
                if (isLocalDead)
                {
                    if (string.IsNullOrEmpty(baseName) && Camouflager.CamouflageTimer > 0f)
                        baseName = target.Data.DefaultOutfit?.PlayerName ?? "";
                    else if (target.CurrentOutfitType == PlayerOutfitType.Shapeshifted)
                        baseName = $"{target.CurrentOutfit?.PlayerName} ({target.Data.DefaultOutfit?.PlayerName})";
                }

                TAG_STRING_BUILDER.Clear();
                TAG_STRING_BUILDER.Append(baseName);

                // Role Tags (Meeting Only)
                var r = PlayerRole.GetRole(target);
                if (r != null && !string.IsNullOrEmpty(r.NameTag)) TAG_STRING_BUILDER.Append(r.NameTag);

                // Detective / Hacker
                if (MapSettings.ShowLighterDarker && meetingShow)
                    TAG_STRING_BUILDER.Append(" (")
                                      .Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId)
                                                  ? Tr.Get(TrKey.DetectiveLightLabel)
                                                  : Tr.Get(TrKey.DetectiveDarkLabel))
                                      .Append(')');

                // Lovers
                if (Lovers.IsLovers(target) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead)))
                    TAG_STRING_BUILDER.Append(Lovers.GetIcon(target));

                var resultText = TAG_STRING_BUILDER.ToString();
                if (pva.NameText.text != resultText) pva.NameText.text = resultText;

                if (COLOR_CACHE.TryGetValue(target.PlayerId, out var c))
                {
                    if (pva.NameText.color != c)
                        pva.NameText.color = c;
                }
            }
        }

        _isUpdating = false;
    }

    public static void SetPlayerNameColor(PlayerControl p, Color color)
    {
        if (_isUpdating)
        {
            COLOR_CACHE[p.PlayerId] = color;
            return;
        }

        p.cosmetics.nameText.color = color;
        if (MeetingHud.Instance != null)
        {
            foreach (var player in MeetingHud.Instance.playerStates)
            {
                if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                    player.NameText.color = color;
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
                if (roleInstance != null) SetPlayerNameColor(lp, roleInstance.RoleColor);

                foreach (var mod in PlayerModifier.GetModifiers(lp)) SetPlayerNameColor(lp, mod.ModifierColor);

                // 2. Process logic-heavy vision (Jackal seeing Sidekick, Spy seeing Impostors, etc.)
                foreach (var r in PlayerRole.AllRoles) r.OnUpdateNameColors();
                foreach (var m in PlayerModifier.AllModifiers) m.OnUpdateNameColors();
                break;

            case CustomGameMode.CaptureTheFlag:
                if (CaptureTheFlag.StealerPlayer != null)
                    SetPlayerNameColor(CaptureTheFlag.StealerPlayer, Palette.PlayerColors[15]);

                foreach (var redplayer in CaptureTheFlag.RedteamFlag)
                {
                    if (redplayer != null)
                        SetPlayerNameColor(redplayer, Palette.PlayerColors[0]);
                }

                foreach (var blueplayer in CaptureTheFlag.BlueteamFlag)
                {
                    if (blueplayer != null)
                        SetPlayerNameColor(blueplayer, Palette.PlayerColors[1]);
                }

                break;

            case CustomGameMode.PoliceAndThieves:
                foreach (var policeplayer in PoliceAndThief.PoliceTeam)
                {
                    if (policeplayer != null)
                    {
                        if ((PoliceAndThief.Policeplayer02 != null && policeplayer == PoliceAndThief.Policeplayer02)
                            || (PoliceAndThief.Policeplayer04 != null && policeplayer == PoliceAndThief.Policeplayer04))
                            SetPlayerNameColor(policeplayer, Palette.PlayerColors[5]);
                        else
                            SetPlayerNameColor(policeplayer, Palette.PlayerColors[10]);
                    }
                }

                foreach (var thiefplayer in PoliceAndThief.ThiefTeam)
                {
                    if (thiefplayer != null)
                        SetPlayerNameColor(thiefplayer, Palette.PlayerColors[16]);
                }

                break;

            case CustomGameMode.HotPotato:
                foreach (var notpotatoplayer in HotPotato.NOT_POTATO_TEAM)
                {
                    if (notpotatoplayer != null)
                        SetPlayerNameColor(notpotatoplayer, Palette.PlayerColors[10]);
                }

                foreach (var explodedpotatoplayer in HotPotato.EXPLODED_POTATO_TEAM)
                {
                    if (explodedpotatoplayer != null)
                        SetPlayerNameColor(explodedpotatoplayer, Palette.PlayerColors[9]);
                }

                if (HotPotato.HotPotatoPlayer != null)
                    SetPlayerNameColor(HotPotato.HotPotatoPlayer, Palette.PlayerColors[15]);

                break;

            case CustomGameMode.BattleRoyale:
                if (BattleRoyale.MatchType == 0)
                {
                    foreach (var soloPlayer in BattleRoyale.SoloPlayerTeam)
                    {
                        if (soloPlayer != null)
                            SetPlayerNameColor(soloPlayer, Palette.PlayerColors[2]);
                    }
                }
                else
                {
                    if (BattleRoyale.SerialKiller != null)
                        SetPlayerNameColor(BattleRoyale.SerialKiller, Palette.PlayerColors[15]);

                    foreach (var limeplayer in BattleRoyale.LimeTeam)
                    {
                        if (limeplayer != null)
                            SetPlayerNameColor(limeplayer, Palette.PlayerColors[11]);
                    }

                    foreach (var pinkplayer in BattleRoyale.PinkTeam)
                    {
                        if (pinkplayer != null)
                            SetPlayerNameColor(pinkplayer, Palette.PlayerColors[13]);
                    }
                }

                break;
        }
    }

    private static void SetNameTags()
    {
        switch (MapSettings.GameMode)
        {
            case CustomGameMode.BattleRoyale:
                // BR Lives
                if (BattleRoyale.MatchType == 0)
                {
                    if (PlayerControl.LocalPlayer.Data.IsDead)
                    {
                        if (BattleRoyale.SoloPlayer01 != null)
                            BattleRoyale.SoloPlayer01.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer01Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer02 != null)
                            BattleRoyale.SoloPlayer02.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer02Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer03 != null)
                            BattleRoyale.SoloPlayer03.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer03Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer04 != null)
                            BattleRoyale.SoloPlayer04.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer04Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer05 != null)
                            BattleRoyale.SoloPlayer05.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer05Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer06 != null)
                            BattleRoyale.SoloPlayer06.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer06Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer07 != null)
                            BattleRoyale.SoloPlayer07.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer07Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer08 != null)
                            BattleRoyale.SoloPlayer08.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer08Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer09 != null)
                            BattleRoyale.SoloPlayer09.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer09Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer10 != null)
                            BattleRoyale.SoloPlayer10.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer10Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer11 != null)
                            BattleRoyale.SoloPlayer11.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer11Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer12 != null)
                            BattleRoyale.SoloPlayer12.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer12Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer13 != null)
                            BattleRoyale.SoloPlayer13.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer13Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer14 != null)
                            BattleRoyale.SoloPlayer14.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer14Lifes + "♥)");

                        if (BattleRoyale.SoloPlayer15 != null)
                            BattleRoyale.SoloPlayer15.cosmetics.nameText.text +=
                                Helpers.Cs(BattleRoyale.SoloPlayerColor, " (" + BattleRoyale.SoloPlayer15Lifes + "♥)");
                    }
                    else
                    {
                        if (BattleRoyale.SoloPlayer01 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer01)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer01Lifes + "♥)");
                            BattleRoyale.SoloPlayer01.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer02 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer02)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer02Lifes + "♥)");
                            BattleRoyale.SoloPlayer02.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer03 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer03)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer03Lifes + "♥)");
                            BattleRoyale.SoloPlayer03.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer04 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer04)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer04Lifes + "♥)");
                            BattleRoyale.SoloPlayer04.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer05 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer05)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer05Lifes + "♥)");
                            BattleRoyale.SoloPlayer05.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer06 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer06)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer06Lifes + "♥)");
                            BattleRoyale.SoloPlayer06.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer07 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer07)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer07Lifes + "♥)");
                            BattleRoyale.SoloPlayer07.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer08 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer08)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer08Lifes + "♥)");
                            BattleRoyale.SoloPlayer08.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer09 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer09)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer09Lifes + "♥)");
                            BattleRoyale.SoloPlayer09.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer10 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer10)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer10Lifes + "♥)");
                            BattleRoyale.SoloPlayer10.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer11 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer11)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer11Lifes + "♥)");
                            BattleRoyale.SoloPlayer11.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer12 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer12)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer12Lifes + "♥)");
                            BattleRoyale.SoloPlayer12.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer13 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer13)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer13Lifes + "♥)");
                            BattleRoyale.SoloPlayer13.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer14 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer14)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer14Lifes + "♥)");
                            BattleRoyale.SoloPlayer14.cosmetics.nameText.text += suffix;
                        }

                        if (BattleRoyale.SoloPlayer15 != null && PlayerControl.LocalPlayer == BattleRoyale.SoloPlayer15)
                        {
                            var suffix = Helpers.Cs(BattleRoyale.SoloPlayerColor,
                                                    " (" + BattleRoyale.SoloPlayer15Lifes + "♥)");
                            BattleRoyale.SoloPlayer15.cosmetics.nameText.text += suffix;
                        }
                    }
                }
                else
                {
                    foreach (var limePlayer in BattleRoyale.LimeTeam)
                    {
                        if (limePlayer == PlayerControl.LocalPlayer)
                        {
                            if (BattleRoyale.LimePlayer01 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer01Lifes + "♥)");
                                BattleRoyale.LimePlayer01.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer02 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer02Lifes + "♥)");
                                BattleRoyale.LimePlayer02.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer03 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer03Lifes + "♥)");
                                BattleRoyale.LimePlayer03.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer04 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer04Lifes + "♥)");
                                BattleRoyale.LimePlayer04.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer05 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer05Lifes + "♥)");
                                BattleRoyale.LimePlayer05.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer06 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer06Lifes + "♥)");
                                BattleRoyale.LimePlayer06.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.LimePlayer07 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.LimeTeamColor,
                                                        " (" + BattleRoyale.LimePlayer07Lifes + "♥)");
                                BattleRoyale.LimePlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }

                    foreach (var pinkPlayer in BattleRoyale.PinkTeam)
                    {
                        if (pinkPlayer == PlayerControl.LocalPlayer)
                        {
                            if (BattleRoyale.PinkPlayer01 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer01Lifes + "♥)");
                                BattleRoyale.PinkPlayer01.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer02 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer02Lifes + "♥)");
                                BattleRoyale.PinkPlayer02.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer03 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer03Lifes + "♥)");
                                BattleRoyale.PinkPlayer03.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer04 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer04Lifes + "♥)");
                                BattleRoyale.PinkPlayer04.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer05 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer05Lifes + "♥)");
                                BattleRoyale.PinkPlayer05.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer06 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer06Lifes + "♥)");
                                BattleRoyale.PinkPlayer06.cosmetics.nameText.text += suffix;
                            }

                            if (BattleRoyale.PinkPlayer07 != null)
                            {
                                var suffix = Helpers.Cs(BattleRoyale.PinkTeamColor,
                                                        " (" + BattleRoyale.PinkPlayer07Lifes + "♥)");
                                BattleRoyale.PinkPlayer07.cosmetics.nameText.text += suffix;
                            }
                        }
                    }

                    if (BattleRoyale.SerialKiller != null && PlayerControl.LocalPlayer == BattleRoyale.SerialKiller)
                    {
                        var suffix = Helpers.Cs(BattleRoyale.SerialKillerColor,
                                                " (" + BattleRoyale.SerialKillerLifes + "♥)");
                        BattleRoyale.SerialKiller.cosmetics.nameText.text += suffix;
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

        var enabled = Helpers.ShowButtons;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Vampire))
            enabled &= false;
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanKill)
            enabled &= false;
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Janitor)) enabled &= false;

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

    public static void UpdateReportButton(HudManager __instance)
    {
        if (__instance?.ReportButton == null) return;
        if (MeetingHud.Instance) __instance.ReportButton.Hide();

        if (MapSettings.GameMode is CustomGameMode.Roles)
        {
            if (!ActivatedReportButtonAfterCustomMode)
            {
                __instance.ReportButton.Show();
                ActivatedReportButtonAfterCustomMode = true;
            }

            return;
        }

        var enabled = true;
        if (MapSettings.GameMode is not CustomGameMode.Roles) enabled = false;

        enabled &= __instance.ReportButton.isActiveAndEnabled;

        if (enabled)
            __instance.ReportButton.Show();
        else
            __instance.ReportButton.Hide();
    }

    public static void StopCooldown(PlayerControl __instance)
    {
        if (CustomOptionHolder.StopCooldownOnFixingElecSabotage.GetBool())
        {
            if (Helpers.IsOnElecTask())
                __instance.SetKillTimer(__instance.killTimer + Time.fixedDeltaTime);
        }
    }

    public static void ImpostorSetTarget()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || !localPlayer.Data.Role.IsImpostor || !localPlayer.CanMove || localPlayer.Data.IsDead)
        {
            if (FastDestroyableSingleton<HudManager>.Instance)
                FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

            return;
        }

        var specialTeamRedExists = false;
        if (Spy.Exists)
            specialTeamRedExists = true;
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
                target = Helpers.SetTarget(false, true);
            else
            {
                List<PlayerControl> listP = [.. Spy.AllPlayers];
                foreach (var sidekick in Sidekick.Players)
                {
                    if (sidekick.WasTeamRed)
                        listP.Add(sidekick.Player);
                }

                foreach (var jackal in Jackal.Players)
                {
                    if (jackal.WasTeamRed)
                        listP.Add(jackal.Player);
                }

                target = Helpers.SetTarget(true, true, listP);
            }
        }
        else
            target = Helpers.SetTarget(true, true);

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
    }

    public static void PlayerSizeUpdate(PlayerControl p)
    {
        if (p == null) return;

        var collider = p.GetComponent<CircleCollider2D>();
        if (collider == null) return;

        p.transform.localScale = new(0.7f, 0.7f, 1f);
        collider.radius = Mini.DEFAULT_COLLIDER_RADIUS;
        collider.offset = Mini.DEFAULT_COLLIDER_OFFSET * Vector2.down;

        // Set adapted player size to Mini and Morphing
        if (Camouflager.CamouflageTimer > 0f) return;

        Mini miniRole = null;
        if (p.HasModifier(ModifierType.Mini))
            miniRole = Mini.GetModifier(p);
        else if (Morphing.Exists
                 && p.IsRole(RoleType.Morphing)
                 && Morphing.MorphTimer > 0f
                 && Morphing.MorphTarget != null
                 && Morphing.MorphTarget.HasModifier(ModifierType.Mini))
            miniRole = Mini.GetModifier(Morphing.MorphTarget);

        if (miniRole != null)
        {
            var growingProgress = miniRole.GrowingProgress();
            var scale = (growingProgress * 0.35f) + 0.35f;
            var correctedColliderRadius = (Mini.DEFAULT_COLLIDER_RADIUS * 0.7f) / scale;

            p.transform.localScale = new(scale, scale, 1f);
            collider.radius = correctedColliderRadius;
        }
    }

    public static void CamouflageAndMorphActions()
    {
        var oldCamouflageTimer = Camouflager.CamouflageTimer;
        var oldMorphTimer = Morphing.MorphTimer;

        Camouflager.CamouflageTimer -= Time.deltaTime;
        Morphing.MorphTimer -= Time.deltaTime;

        // Everyone but morphing reset
        if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f) Camouflager.ResetCamouflage();

        // Morphing reset
        if (oldMorphTimer > 0f && Morphing.MorphTimer <= 0f) Morphing.ResetMorph();
    }

    public static void TimerUpdate()
    {
        var deltaTime = Time.deltaTime;

        switch (MapSettings.GameMode)
        {
            case CustomGameMode.CaptureTheFlag:
                // CTF timers
                RebuildUs.ProgressStart += deltaTime;
                MapSettings.GamemodeMatchDuration -= deltaTime;
                if (RebuildUs.Progress != null)
                {
                    RebuildUs.Progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.GamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.Progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.ProgressStart / RebuildUs.ProgressEnd);
                }

                if (MapSettings.GamemodeMatchDuration <= 0)
                {
                    // both teams with same points = Draw
                    if (CaptureTheFlag.CurrentRedTeamPoints == CaptureTheFlag.CurrentBlueTeamPoints)
                    {
                        CaptureTheFlag.TriggerDrawWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.DrawTeamWin, false);
                    }
                    // Red team more points than blue team = red team win
                    else if (CaptureTheFlag.CurrentRedTeamPoints > CaptureTheFlag.CurrentBlueTeamPoints)
                    {
                        CaptureTheFlag.TriggerRedTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.RedTeamFlagWin, false);
                    }
                    // otherwise blue team win
                    else
                    {
                        CaptureTheFlag.TriggerBlueTeamWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BlueTeamFlagWin, false);
                    }
                }

                break;
            case CustomGameMode.PoliceAndThieves:
                // PT timers
                PoliceAndThief.Policeplayer01LightTimer -= deltaTime;
                PoliceAndThief.Policeplayer02LightTimer -= deltaTime;
                PoliceAndThief.Policeplayer03LightTimer -= deltaTime;
                PoliceAndThief.Policeplayer04LightTimer -= deltaTime;
                PoliceAndThief.Policeplayer05LightTimer -= deltaTime;
                PoliceAndThief.Policeplayer06LightTimer -= deltaTime;

                RebuildUs.ProgressStart += deltaTime;
                MapSettings.GamemodeMatchDuration -= deltaTime;
                if (RebuildUs.Progress != null)
                {
                    RebuildUs.Progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.GamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.Progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.ProgressStart / RebuildUs.ProgressEnd);
                }

                if (MapSettings.GamemodeMatchDuration <= 0)
                {
                    PoliceAndThief.TriggerPoliceWin = true;
                    GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.ThiefModePoliceWin, false);
                }

                break;
            case CustomGameMode.HotPotato:
                // HP timers
                if (HotPotato.FirstPotatoTransfered)
                {
                    HotPotato.TimeforTransfer -= deltaTime;

                    if (HotPotato.TimeforTransfer <= 0
                        && !HotPotato.HotPotatoPlayer.Data.IsDead
                        && AmongUsClient.Instance.AmHost)
                    {
                        // Ensure host send an RPC so the time doesn't bug
                        var winWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.HotPotatoExploded, SendOption.Reliable);
                        AmongUsClient.Instance.FinishRpcImmediately(winWriter);
                        RPCProcedure.HotPotatoExploded();
                    }

                    RebuildUs.ProgressStart += deltaTime;
                    MapSettings.GamemodeMatchDuration -= deltaTime;
                    if (RebuildUs.Progress != null)
                    {
                        RebuildUs.Progress.GetComponentInChildren<TextMeshPro>().text =
                            new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                                  .Append("</color>")
                                                                  .Append(MapSettings.GamemodeMatchDuration
                                                                              .ToString("F0"))
                                                                  .Append(" | <color=#FF8000FF>")
                                                                  .Append(Tr.Get(TrKey.HotPotatoStatus))
                                                                  .Append("</color>")
                                                                  .Append(HotPotato.TimeforTransfer.ToString("F0"))
                                                                  .ToString();
                        RebuildUs.Progress.GetComponent<ProgressTracker>().curValue =
                            Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                       RebuildUs.ProgressStart / RebuildUs.ProgressEnd);
                    }

                    if (MapSettings.GamemodeMatchDuration <= 0)
                    {
                        HotPotato.TriggerHotPotatoEnd = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.HotPotatoEnd, false);
                    }
                }

                break;
            case CustomGameMode.BattleRoyale:
                // BR timers
                RebuildUs.ProgressStart += deltaTime;
                MapSettings.GamemodeMatchDuration -= deltaTime;
                if (RebuildUs.Progress != null)
                {
                    RebuildUs.Progress.GetComponentInChildren<TextMeshPro>().text =
                        new StringBuilder("<color=#FF8000FF>").Append(Tr.Get(TrKey.TimeLeft))
                                                              .Append("</color>")
                                                              .Append(MapSettings.GamemodeMatchDuration.ToString("F0"))
                                                              .ToString();
                    RebuildUs.Progress.GetComponent<ProgressTracker>().curValue =
                        Mathf.Lerp(PlayerControl.AllPlayerControls.Count - 1, 0,
                                   RebuildUs.ProgressStart / RebuildUs.ProgressEnd);
                }

                if (MapSettings.GamemodeMatchDuration <= 0)
                {
                    if (BattleRoyale.MatchType == 2)
                    {
                        if (BattleRoyale.SerialKiller != null)
                        {
                            // all teams with same points = Draw
                            if (BattleRoyale.LimePoints == BattleRoyale.PinkPoints
                                && BattleRoyale.PinkPoints == BattleRoyale.SerialKillerPoints)
                            {
                                BattleRoyale.TriggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                            // Lime team more points than pink team and serial killer = lime team win
                            else if (BattleRoyale.LimePoints > BattleRoyale.PinkPoints
                                     && BattleRoyale.LimePoints > BattleRoyale.SerialKillerPoints)
                            {
                                BattleRoyale.TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleLimeTeamWin, false);
                            }
                            // otherwise pink team win
                            else if (BattleRoyale.PinkPoints > BattleRoyale.LimePoints
                                     && BattleRoyale.PinkPoints > BattleRoyale.SerialKillerPoints)
                            {
                                BattleRoyale.TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyalePinkTeamWin, false);
                            }
                            // otherwise serial killer win
                            else if (BattleRoyale.SerialKillerPoints > BattleRoyale.LimePoints
                                     && BattleRoyale.SerialKillerPoints > BattleRoyale.PinkPoints)
                            {
                                BattleRoyale.TriggerSerialKillerWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleSerialKillerWin, false);
                            }
                            // draw between some of the teams
                            else
                            {
                                BattleRoyale.TriggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                        }
                        else
                        {
                            // both teams with same points = Draw
                            if (BattleRoyale.LimePoints == BattleRoyale.PinkPoints)
                            {
                                BattleRoyale.TriggerDrawWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleDraw,
                                                                false);
                            }
                            // Lime team more points than pink team = lime team win
                            else if (BattleRoyale.LimePoints > BattleRoyale.PinkPoints)
                            {
                                BattleRoyale.TriggerLimeTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyaleLimeTeamWin, false);
                            }
                            // otherwise pink team win
                            else
                            {
                                BattleRoyale.TriggerPinkTeamWin = true;
                                GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason
                                                                    .BattleRoyalePinkTeamWin, false);
                            }
                        }
                    }
                    else
                    {
                        BattleRoyale.TriggerTimeWin = true;
                        GameManager.Instance.RpcEndGame((GameOverReason)CustomGameOverReason.BattleRoyaleTimeWin,
                                                        false);
                    }
                }

                break;
        }
    }
}
