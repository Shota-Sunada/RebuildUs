namespace RebuildUs.Patches;

public static class Update
{
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

    public static void ResetNameTagsAndColors()
    {
        var localPlayer = PlayerControl.LocalPlayer;
        if (localPlayer == null) return;

        var playersById = Helpers.AllPlayersById();
        bool isLocalImpostor = localPlayer.IsTeamImpostor();

        foreach (PlayerControl player in PlayerControl.AllPlayerControls)
        {
            if (player == null || player.cosmetics == null || player.cosmetics.nameText == null) continue;

            string expectedText = Helpers.HidePlayerName(localPlayer, player) ? "" : player.CurrentOutfit.PlayerName;
            Color expectedColor = (isLocalImpostor && player.IsTeamImpostor()) ? Palette.ImpostorRed : Color.white;

            var nameText = player.cosmetics.nameText;
            if (nameText.text != expectedText) nameText.text = expectedText;
            if (nameText.color != expectedColor) nameText.color = expectedColor;
        }

        var meetingHud = MeetingHud.Instance;
        if (meetingHud != null)
        {
            foreach (PlayerVoteArea pva in meetingHud.playerStates)
            {
                if (pva == null || pva.NameText == null) continue;

                if (playersById.TryGetValue((byte)pva.TargetPlayerId, out var playerControl))
                {
                    string expectedText = playerControl.Data.PlayerName;
                    Color expectedColor = (isLocalImpostor && playerControl.Data.Role.IsImpostor) ? Palette.ImpostorRed : Color.white;

                    if (pva.NameText.text != expectedText) pva.NameText.text = expectedText;
                    if (pva.NameText.color != expectedColor) pva.NameText.color = expectedColor;
                }
            }
        }
    }

    public static void SetPlayerNameColor(PlayerControl p, Color color)
    {
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

    public static void SetNameTags()
    {
        var lp = PlayerControl.LocalPlayer;
        if (lp == null) return;

        // 1. Process Roles and Modifiers NameTags
        foreach (var r in PlayerRole.AllRoles)
        {
            if (r.Player == null || r.Player.cosmetics == null || r.Player.cosmetics.nameText == null) continue;
            if (string.IsNullOrEmpty(r.Player.cosmetics.nameText.text)) continue;

            var tag = r.NameTag;
            if (!string.IsNullOrEmpty(tag))
            {
                TagStringBuilder.Clear();
                TagStringBuilder.Append(r.Player.cosmetics.nameText.text).Append(tag);
                r.Player.cosmetics.nameText.text = TagStringBuilder.ToString();
            }
            r.OnUpdateNameTags();
        }

        foreach (var m in PlayerModifier.AllModifiers)
        {
            if (m.Player == null || m.Player.cosmetics == null || m.Player.cosmetics.nameText == null) continue;
            if (string.IsNullOrEmpty(m.Player.cosmetics.nameText.text)) continue;

            var tag = m.NameTag;
            if (!string.IsNullOrEmpty(tag))
            {
                TagStringBuilder.Clear();
                TagStringBuilder.Append(m.Player.cosmetics.nameText.text).Append(tag);
                m.Player.cosmetics.nameText.text = TagStringBuilder.ToString();
            }
            m.OnUpdateNameTags();
        }

        bool meetingShow = MeetingHud.Instance != null && (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion);

        // 2. Voting Area NameTags
        if (MeetingHud.Instance != null)
        {
            var playersById = Helpers.AllPlayersById();
            foreach (var voteArea in MeetingHud.Instance.playerStates)
            {
                if (voteArea == null || voteArea.NameText == null) continue;

                var target = playersById.ContainsKey(voteArea.TargetPlayerId) ? playersById[voteArea.TargetPlayerId] : null;
                if (target == null) continue;

                var targetRole = PlayerRole.GetRole(target);
                if (targetRole != null)
                {
                    var tag = targetRole.NameTag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        TagStringBuilder.Clear();
                        TagStringBuilder.Append(voteArea.NameText.text).Append(tag);
                        voteArea.NameText.text = TagStringBuilder.ToString();
                    }
                }

                // Hacker and Detective Lighter/Darker
                if (ModMapOptions.ShowLighterDarker && meetingShow)
                {
                    TagStringBuilder.Clear();
                    TagStringBuilder.Append(voteArea.NameText.text)
                        .Append(" (")
                        .Append(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId) ? Tr.Get("Hud.DetectiveLightLabel") : Tr.Get("Hud.DetectiveDarkLabel"))
                        .Append(')');
                    voteArea.NameText.text = TagStringBuilder.ToString();
                }
            }
        }

        // 3. Lovers (Special Logic)
        if (lp.IsLovers() && lp.IsAlive())
        {
            string suffix = Lovers.GetIcon(lp);
            var lover1 = lp;
            var lover2 = lp.GetPartner();

            if (lover1.cosmetics?.nameText != null)
            {
                TagStringBuilder.Clear();
                TagStringBuilder.Append(lover1.cosmetics.nameText.text).Append(suffix);
                lover1.cosmetics.nameText.text = TagStringBuilder.ToString();
            }

            if (lover2 != null && lover2.cosmetics?.nameText != null && !Helpers.HidePlayerName(lover2))
            {
                TagStringBuilder.Clear();
                TagStringBuilder.Append(lover2.cosmetics.nameText.text).Append(suffix);
                lover2.cosmetics.nameText.text = TagStringBuilder.ToString();
            }

            if (meetingShow)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    if (player == null || player.NameText == null) continue;
                    if (lover1.PlayerId == player.TargetPlayerId || (lover2 != null && lover2.PlayerId == player.TargetPlayerId))
                    {
                        TagStringBuilder.Clear();
                        TagStringBuilder.Append(player.NameText.text).Append(suffix);
                        player.NameText.text = TagStringBuilder.ToString();
                    }
                }
            }
        }

        if (ModMapOptions.GhostsSeeRoles && lp.IsDead())
        {
            foreach (var couple in Lovers.Couples)
            {
                string suffix = Lovers.GetIcon(couple.Lover1);

                if (couple.Lover1.cosmetics?.nameText != null)
                {
                    TagStringBuilder.Clear();
                    TagStringBuilder.Append(couple.Lover1.cosmetics.nameText.text).Append(suffix);
                    couple.Lover1.cosmetics.nameText.text = TagStringBuilder.ToString();
                }

                if (couple.Lover2.cosmetics?.nameText != null)
                {
                    TagStringBuilder.Clear();
                    TagStringBuilder.Append(couple.Lover2.cosmetics.nameText.text).Append(suffix);
                    couple.Lover2.cosmetics.nameText.text = TagStringBuilder.ToString();
                }

                if (meetingShow)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (player == null || player.NameText == null) continue;
                        if (couple.Lover1.PlayerId == player.TargetPlayerId || couple.Lover2.PlayerId == player.TargetPlayerId)
                        {
                            TagStringBuilder.Clear();
                            TagStringBuilder.Append(player.NameText.text).Append(suffix);
                            player.NameText.text = TagStringBuilder.ToString();
                        }
                    }
                }
            }
        }
    }

    public static void UpdateImpostorKillButton(HudManager __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
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
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    public static void UpdateSabotageButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.SabotageButton.Hide();
    }

    public static void UpdateVentButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
    }

    public static void UpdateReportButton(HudManager __instance)
    {
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