namespace RebuildUs.Patches;

public static class Update
{
    public static void ResetNameTagsAndColors()
    {
        var playersById = Helpers.AllPlayersById();

        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            player.cosmetics.nameText.text = Helpers.HidePlayerName(CachedPlayer.LocalPlayer.PlayerControl, player) ? "" : player.CurrentOutfit.PlayerName;
            if (CachedPlayer.LocalPlayer.PlayerControl.IsTeamImpostor() && player.IsTeamImpostor())
            {
                player.cosmetics.nameText.color = Palette.ImpostorRed;
            }
            else
            {
                player.cosmetics.nameText.color = Color.white;
            }
        }

        if (MeetingHud.Instance != null)
        {
            foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
            {
                PlayerControl playerControl = playersById.ContainsKey((byte)player.TargetPlayerId) ? playersById[(byte)player.TargetPlayerId] : null;
                if (playerControl != null)
                {
                    player.NameText.text = playerControl.Data.PlayerName;
                    if (CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor && playerControl.Data.Role.IsImpostor)
                    {
                        player.NameText.color = Palette.ImpostorRed;
                    }
                    else
                    {
                        player.NameText.color = Color.white;
                    }
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
        var lp = CachedPlayer.LocalPlayer.PlayerControl;
        if (lp == null) return;

        ResetNameTagsAndColors();

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

    public static void SetNameTags()
    {
        var lp = CachedPlayer.LocalPlayer.PlayerControl;
        if (lp == null) return;

        ResetNameTagsAndColors();

        // 1. Process Roles and Modifiers NameTags
        foreach (var r in PlayerRole.AllRoles)
        {
            if (string.IsNullOrEmpty(r.Player.cosmetics.nameText.text)) continue;
            var tag = r.NameTag;
            if (!string.IsNullOrEmpty(tag)) r.Player.cosmetics.nameText.text += tag;
            r.OnUpdateNameTags();
        }

        foreach (var m in PlayerModifier.AllModifiers)
        {
            if (string.IsNullOrEmpty(m.Player.cosmetics.nameText.text)) continue;
            var tag = m.NameTag;
            if (!string.IsNullOrEmpty(tag)) m.Player.cosmetics.nameText.text += tag;
            m.OnUpdateNameTags();
        }

        bool meetingShow = MeetingHud.Instance != null && (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion);

        // 2. Voting Area NameTags
        if (MeetingHud.Instance != null)
        {
            foreach (var voteArea in MeetingHud.Instance.playerStates)
            {
                var target = Helpers.PlayerById(voteArea.TargetPlayerId);
                if (target == null) continue;

                var targetRole = PlayerRole.GetRole(target);
                if (targetRole != null)
                {
                    var tag = targetRole.NameTag;
                    if (!string.IsNullOrEmpty(tag)) voteArea.NameText.text += tag;
                }

                // Hacker and Detective Lighter/Darker
                if (ModMapOptions.ShowLighterDarker && meetingShow)
                {
                    voteArea.NameText.text += $" ({(Helpers.IsLighterColor(target.Data.DefaultOutfit.ColorId) ? Tr.Get("Hud.DetectiveLightLabel") : Tr.Get("Hud.DetectiveDarkLabel"))})";
                }
            }
        }

        // 3. Lovers (Special Logic)
        if (lp.IsLovers() && lp.IsAlive())
        {
            string suffix = Lovers.GetIcon(lp);
            var lover1 = lp;
            var lover2 = lp.GetPartner();

            lover1.cosmetics.nameText.text += suffix;
            if (!Helpers.HidePlayerName(lover2))
                lover2.cosmetics.nameText.text += suffix;

            if (meetingShow)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    if (lover1.PlayerId == player.TargetPlayerId || lover2.PlayerId == player.TargetPlayerId)
                        player.NameText.text += suffix;
                }
            }
        }

        if (ModMapOptions.GhostsSeeRoles && lp.IsDead())
        {
            foreach (var couple in Lovers.Couples)
            {
                string suffix = Lovers.GetIcon(couple.Lover1);
                couple.Lover1.cosmetics.nameText.text += suffix;
                couple.Lover2.cosmetics.nameText.text += suffix;

                if (meetingShow)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (couple.Lover1.PlayerId == player.TargetPlayerId || couple.Lover2.PlayerId == player.TargetPlayerId)
                        {
                            player.NameText.text += suffix;
                        }
                    }
                }
            }
        }
    }

    public static void UpdateImpostorKillButton(HudManager __instance)
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor) return;
        if (MeetingHud.Instance)
        {
            __instance.KillButton.Hide();
            return;
        }
        bool enabled = Helpers.ShowButtons;
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vampire))
        {
            enabled &= false;
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.CanKill)
        {
            enabled &= false;
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Janitor))
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
        if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor || !CachedPlayer.LocalPlayer.PlayerControl.CanMove || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
        {
            // !isImpostor || !canMove || isDead
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        PlayerControl target = null;
        if (Spy.Exists || Sidekick.Players.Any(x => x.WasTeamRed) || Jackal.Players.Any(x => x.WasTeamRed))
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

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
    }

    public static void PlayerSizeUpdate(PlayerControl p)
    {
        // Set default player size
        CircleCollider2D collider = p.GetComponent<CircleCollider2D>();

        p.transform.localScale = new Vector3(0.7f, 0.7f, 1f);
        collider.radius = Mini.DefaultColliderRadius;
        collider.offset = Mini.DefaultColliderOffset * Vector2.down;

        // Set adapted player size to Mini and Morphing
        if (Camouflager.CamouflageTimer > 0f) return;

        foreach (var mini in Mini.Players)
        {
            float growingProgress = mini.GrowingProgress();
            float scale = growingProgress * 0.35f + 0.35f;
            float correctedColliderRadius = Mini.DefaultColliderRadius * 0.7f / scale; // scale / 0.7f is the factor by which we decrease the player size, hence we need to increase the collider size by 0.7f / scale

            if (p.HasModifier(ModifierType.Mini))
            {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
            if (Morphing.Exists && p.IsRole(RoleType.Morphing) && Morphing.MorphTarget.HasModifier(ModifierType.Mini) && Morphing.MorphTimer > 0f)
            {
                p.transform.localScale = new Vector3(scale, scale, 1f);
                collider.radius = correctedColliderRadius;
            }
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
