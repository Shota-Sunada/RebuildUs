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

        foreach (var sourcePlayer in PlayerControl.AllPlayerControls)
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
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p == null) continue;
            Color baseColor = (isLocalImpostor && p.IsTeamImpostor()) ? Palette.ImpostorRed : Color.white;
            SetPlayerNameColor(p, baseColor);
        }

        // 2. Calculate Role/Modifier Colors (populates ColorCache via SetPlayerNameColor)
        SetNameColors();

        // 3. Update Player Instances
        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
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
                if (Lovers.IsLovers(player) && (localPlayer.IsLovers() || (ModMapOptions.GhostsSeeRoles && isLocalDead)))
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
                if (!playersById.TryGetValue(pva.TargetPlayerId, out var target) || target == null || target.Data == null) continue;

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
                if (ModMapOptions.ShowLighterDarker && meetingShow)
                {
                    TagStringBuilder.Append(" (")
                        .Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId) ? Tr.Get(TranslateKey.DetectiveLightLabel) : Tr.Get(TranslateKey.DetectiveDarkLabel))
                        .Append(')');
                }

                // Lovers
                if (Lovers.IsLovers(target) && (localPlayer.IsLovers() || (ModMapOptions.GhostsSeeRoles && isLocalDead)))
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
    }

    private static readonly StringBuilder TagStringBuilder = new();

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

    public static void UpdateReportButton(HudManager __instance)
    {
        if (__instance?.ReportButton == null) return;
        if (MeetingHud.Instance) __instance.ReportButton.Hide();
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

        PlayerControl target = null;
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
        else if (Morphing.Exists && p.IsRole(RoleType.Morphing) && Morphing.MorphTimer > 0f && Morphing.MorphTarget != null && Morphing.MorphTarget.HasModifier(ModifierType.Mini))
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
}