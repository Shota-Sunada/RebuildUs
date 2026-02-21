namespace RebuildUs.Patches;

internal static class Update
{
    private static bool _isUpdating;
    private static readonly Dictionary<byte, Color> ColorCache = [];

    private static readonly StringBuilder TagStringBuilder = new();

    internal static void SetChatBubbleColor(ChatBubble bubble, string playerName)
    {
        if (bubble == null) return;
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp == null || !lp.IsTeamImpostor()) return;

        foreach (PlayerControl sourcePlayer in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (sourcePlayer.Data == null || !sourcePlayer.Data.PlayerName.Equals(playerName)) continue;
            if (sourcePlayer.IsRole(RoleType.Spy)
                || (sourcePlayer.IsRole(RoleType.Sidekick) && Sidekick.GetRole(sourcePlayer)?.WasTeamRed == true)
                || (sourcePlayer.IsRole(RoleType.Jackal) && Jackal.GetRole(sourcePlayer)?.WasTeamRed == true))
                bubble.NameText.color = Palette.ImpostorRed;

            break;
        }
    }

    internal static void UpdatePlayerNamesAndColors()
    {
        _isUpdating = true;
        ColorCache.Clear();

        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null)
        {
            _isUpdating = false;
            return;
        }

        bool isLocalImpostor = localPlayer.IsTeamImpostor();
        bool isLocalDead = localPlayer.IsDead();
        bool meetingShow = Helpers.ShowMeetingText;

        // 1. Initialize Base Colors
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (p == null) continue;
            Color baseColor = isLocalImpostor && p.IsTeamImpostor() ? Palette.ImpostorRed : Color.white;
            SetPlayerNameColor(p, baseColor);
        }

        // 2. Calculate Role/Modifier Colors (populates ColorCache via SetPlayerNameColor)
        SetNameColors();

        // 3. Update Player Instances
        foreach (PlayerControl player in PlayerControl.AllPlayerControls.GetFastEnumerator())
        {
            if (player == null || player.cosmetics == null || player.cosmetics.nameText == null) continue;

            // --- Name Calculation ---
            string finalName = player.CurrentOutfit.PlayerName;
            bool hideName = Helpers.HidePlayerName(localPlayer, player);

            if (hideName)
                finalName = "";
            else if (isLocalDead)
            {
                if (string.IsNullOrEmpty(finalName) && Camouflager.CamouflageTimer > 0f)
                    finalName = player.Data.DefaultOutfit.PlayerName;
                else if (player.CurrentOutfitType == PlayerOutfitType.Shapeshifted) finalName = $"{player.CurrentOutfit.PlayerName} ({player.Data.DefaultOutfit.PlayerName})";
            }

            TagStringBuilder.Clear();
            TagStringBuilder.Append(finalName);

            if (!string.IsNullOrEmpty(finalName))
            {
                // Role Tags
                PlayerRole r = PlayerRole.GetRole(player);
                if (r != null)
                {
                    if (!string.IsNullOrEmpty(r.NameTag)) TagStringBuilder.Append(r.NameTag);
                    r.OnUpdateNameTags();
                }

                // Modifier Tags
                foreach (PlayerModifier m in PlayerModifier.GetModifiers(player))
                {
                    if (!string.IsNullOrEmpty(m.NameTag)) TagStringBuilder.Append(m.NameTag);
                    m.OnUpdateNameTags();
                }

                // Lovers
                if (Lovers.IsLovers(player) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead))) TagStringBuilder.Append(Lovers.GetIcon(player));
            }

            string resultText = TagStringBuilder.ToString();
            if (player.cosmetics.nameText.text != resultText) player.cosmetics.nameText.text = resultText;

            if (!ColorCache.TryGetValue(player.PlayerId, out Color c)) continue;
            if (player.cosmetics.nameText.color != c) player.cosmetics.nameText.color = c;
        }

        // 4. Update Meeting HUD
        if (MeetingHud.Instance != null)
        {
            Dictionary<byte, PlayerControl> playersById = Helpers.AllPlayersById();
            foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
            {
                if (pva == null || pva.NameText == null) continue;
                if (!playersById.TryGetValue(pva.TargetPlayerId, out PlayerControl target) || target == null || target.Data == null) continue;

                string baseName = target.Data.PlayerName;
                if (isLocalDead)
                {
                    if (string.IsNullOrEmpty(baseName) && Camouflager.CamouflageTimer > 0f)
                        baseName = target.Data.DefaultOutfit?.PlayerName ?? "";
                    else if (target.CurrentOutfitType == PlayerOutfitType.Shapeshifted) baseName = $"{target.CurrentOutfit?.PlayerName} ({target.Data.DefaultOutfit?.PlayerName})";
                }

                TagStringBuilder.Clear();
                TagStringBuilder.Append(baseName);

                // Role Tags (Meeting Only)
                PlayerRole r = PlayerRole.GetRole(target);
                if (r != null && !string.IsNullOrEmpty(r.NameTag)) TagStringBuilder.Append(r.NameTag);

                // Detective / Hacker
                if (MapSettings.ShowLighterDarker && meetingShow) TagStringBuilder.Append(" (").Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId) ? Tr.Get(TrKey.DetectiveLightLabel) : Tr.Get(TrKey.DetectiveDarkLabel)).Append(')');

                // Lovers
                if (Lovers.IsLovers(target) && (localPlayer.IsLovers() || (MapSettings.GhostsSeeRoles && isLocalDead))) TagStringBuilder.Append(Lovers.GetIcon(target));

                string resultText = TagStringBuilder.ToString();
                if (pva.NameText.text != resultText) pva.NameText.text = resultText;

                if (!ColorCache.TryGetValue(target.PlayerId, out Color c)) continue;
                if (pva.NameText.color != c) pva.NameText.color = c;
            }
        }

        _isUpdating = false;
    }

    internal static void SetPlayerNameColor(PlayerControl p, Color color)
    {
        if (_isUpdating)
        {
            ColorCache[p.PlayerId] = color;
            return;
        }

        p.cosmetics.nameText.color = color;
        if (MeetingHud.Instance == null) return;
        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
            if (player.NameText != null && p.PlayerId == player.TargetPlayerId)
                player.NameText.color = color;
    }

    private static void SetNameColors()
    {
        PlayerControl lp = PlayerControl.LocalPlayer;
        if (lp == null) return;

        // 1. Set Local Player Color
        PlayerRole roleInstance = PlayerRole.GetRole(lp);
        if (roleInstance != null) SetPlayerNameColor(lp, roleInstance.RoleColor);

        foreach (PlayerModifier mod in PlayerModifier.GetModifiers(lp)) SetPlayerNameColor(lp, mod.ModifierColor);

        // 2. Process logic-heavy vision (Jackal seeing Sidekick, Spy seeing Impostors, etc.)
        foreach (PlayerRole r in PlayerRole.AllRoles) r.OnUpdateNameColors();
        foreach (PlayerModifier m in PlayerModifier.AllModifiers) m.OnUpdateNameColors();
    }

    internal static void UpdateImpostorKillButton(HudManager __instance)
    {
        if (__instance == null || __instance.KillButton == null) return;
        if (PlayerControl.LocalPlayer?.Data?.Role?.IsImpostor != true) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }

        bool enabled = Helpers.ShowButtons;
        if (PlayerControl.LocalPlayer.IsRole(RoleType.Vampire)
            || (PlayerControl.LocalPlayer.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanKill)
            || PlayerControl.LocalPlayer.IsRole(RoleType.Janitor))
            enabled = false;

        if (enabled) __instance.KillButton.Show();
        else __instance.KillButton.Hide();
    }

    internal static void UpdateUseButton(HudManager __instance)
    {
        if (__instance?.UseButton == null) return;
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    internal static void UpdateSabotageButton(HudManager __instance)
    {
        if (__instance?.SabotageButton == null) return;
        if (MeetingHud.Instance) __instance.SabotageButton.Hide();
    }

    internal static void UpdateVentButton(HudManager __instance)
    {
        if (__instance?.ImpostorVentButton == null) return;
        if (MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
    }

    internal static void UpdateReportButton(HudManager __instance)
    {
        if (__instance?.ReportButton == null) return;
        if (MeetingHud.Instance) __instance.ReportButton.Hide();
    }

    internal static void StopCooldown(PlayerControl __instance)
    {
        if (!CustomOptionHolder.StopCooldownOnFixingElecSabotage.GetBool()) return;
        if (Helpers.IsOnElecTask()) __instance.SetKillTimer(__instance.killTimer + Time.fixedDeltaTime);
    }

    internal static void ImpostorSetTarget()
    {
        PlayerControl localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null || !localPlayer.Data.Role.IsImpostor || !localPlayer.CanMove || localPlayer.Data.IsDead)
        {
            if (FastDestroyableSingleton<HudManager>.Instance) FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);

            return;
        }

        bool specialTeamRedExists = false;
        if (Spy.Exists)
            specialTeamRedExists = true;
        else
        {
            foreach (Sidekick sk in Sidekick.Players)
            {
                if (sk.WasTeamRed)
                {
                    specialTeamRedExists = true;
                    break;
                }
            }

            if (!specialTeamRedExists)
            {
                foreach (Jackal jk in Jackal.Players)
                {
                    if (!jk.WasTeamRed) continue;
                    specialTeamRedExists = true;
                    break;
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
                foreach (Sidekick sidekick in Sidekick.Players)
                    if (sidekick.WasTeamRed)
                        listP.Add(sidekick.Player);

                foreach (Jackal jackal in Jackal.Players)
                    if (jackal.WasTeamRed)
                        listP.Add(jackal.Player);

                target = Helpers.SetTarget(true, true, listP);
            }
        }
        else
            target = Helpers.SetTarget(true, true);

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target);
    }

    internal static void PlayerSizeUpdate(PlayerControl p)
    {
        if (p == null) return;

        CircleCollider2D collider = p.GetComponent<CircleCollider2D>();
        if (collider == null) return;

        p.transform.localScale = new(0.7f, 0.7f, 1f);
        collider.radius = Mini.DEFAULT_COLLIDER_RADIUS;
        collider.offset = Mini.DEFAULT_COLLIDER_OFFSET * Vector2.down;

        // Set adapted player size to Mini and Morphing
        if (Camouflager.CamouflageTimer > 0f) return;

        Mini miniRole = null;
        if (p.HasModifier(ModifierType.Mini))
            miniRole = Mini.GetModifier(p);
        else if (Morphing.Exists && p.IsRole(RoleType.Morphing) && Morphing.MorphTimer > 0f && Morphing.MorphTarget != null && Morphing.MorphTarget.HasModifier(ModifierType.Mini)) miniRole = Mini.GetModifier(Morphing.MorphTarget);

        if (miniRole == null) return;
        float growingProgress = miniRole.GrowingProgress();
        float scale = (growingProgress * 0.35f) + 0.35f;
        float correctedColliderRadius = (Mini.DEFAULT_COLLIDER_RADIUS * 0.7f) / scale;

        p.transform.localScale = new(scale, scale, 1f);
        collider.radius = correctedColliderRadius;
    }

    internal static void CamouflageAndMorphActions()
    {
        float oldCamouflageTimer = Camouflager.CamouflageTimer;
        float oldMorphTimer = Morphing.MorphTimer;

        Camouflager.CamouflageTimer -= Time.deltaTime;
        Morphing.MorphTimer -= Time.deltaTime;

        // Everyone but morphing reset
        if (oldCamouflageTimer > 0f && Camouflager.CamouflageTimer <= 0f) Camouflager.ResetCamouflage();

        // Morphing reset
        if (oldMorphTimer > 0f && Morphing.MorphTimer <= 0f) Morphing.ResetMorph();
    }
}