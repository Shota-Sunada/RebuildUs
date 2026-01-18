using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;
using RebuildUs.Roles.Modifier;
using RebuildUs.Roles.Neutral;

namespace RebuildUs.Patches;

public static class Update
{
    public static void resetNameTagsAndColors()
    {
        var playersById = Helpers.allPlayersById();

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

    private static void setPlayerNameColor(PlayerControl p, Color color)
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

    public static void setNameColors()
    {
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Jester))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Jester.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mayor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Mayor.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Engineer))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Engineer.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Sheriff))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Sheriff.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Lighter))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Lighter.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Detective))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Detective.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.TimeMaster))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, TimeMaster.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Medic.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Shifter))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Shifter.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Swapper))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Swapper.swapper.Data.Role.IsImpostor ? Palette.ImpostorRed : Swapper.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Seer))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Seer.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Hacker.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Tracker.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Snitch))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Snitch.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Jackal))
        {
            // Jackal can see his sidekick
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Jackal.RoleColor);
            var jackal = Jackal.GetRole();
            if (Sidekick.Exists)
            {
                setPlayerNameColor(jackal.MySidekick, Jackal.RoleColor);
            }
            if (jackal.FakeSidekick != null)
            {
                setPlayerNameColor(jackal.FakeSidekick, Jackal.RoleColor);
            }
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Spy))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Spy.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.SecurityGuard))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, SecurityGuard.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Arsonist))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Arsonist.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.NiceGuesser))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Guesser.NiceGuesser.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.EvilGuesser))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Palette.ImpostorRed);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Bait))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Bait.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Opportunist))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Opportunist.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Vulture))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Vulture.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medium))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Medium.RoleColor);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.PlagueDoctor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, PlagueDoctor.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.FortuneTeller) && (FortuneTeller.isCompletedNumTasks(CachedPlayer.LocalPlayer.PlayerControl) || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, FortuneTeller.color);
        }

        if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.Madmate))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.ModifierColor);

            if (Madmate.knowsImpostors(CachedPlayer.LocalPlayer.PlayerControl))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                    {
                        setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.CreatedMadmate))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.ModifierColor);

            if (CreatedMadmate.knowsImpostors(CachedPlayer.LocalPlayer.PlayerControl))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.GetRole(p).WasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.GetRole(p).WasTeamRed))
                    {
                        setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.LastImpostor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, LastImpostor.color);
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.Munou) && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Munou.color);
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.HasModifier(ModifierType.AntiTeleport) && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, AntiTeleport.color);
        }

        if (GM.gm != null)
        {
            setPlayerNameColor(GM.gm, GM.color);
        }

        // No else if here, as a Lover of team Jackal needs the colors
        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Sidekick))
        {
            // Sidekick can see the jackal
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Jackal.RoleColor);
            if (Jackal.Exists)
            {
                setPlayerNameColor(Jackal.jackal, Jackal.RoleColor);
            }
        }

        // No else if here, as the Impostors need the Spy name to be colored
        if (Spy.spy != null && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            setPlayerNameColor(Spy.spy, Spy.color);
        }
        if (Sidekick.sidekick != null && Sidekick.wasTeamRed && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            setPlayerNameColor(Sidekick.sidekick, Spy.color);
        }
        if (Jackal.jackal != null && Jackal.wasTeamRed && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            setPlayerNameColor(Jackal.jackal, Spy.color);
        }

        // Crewmate roles with no changes: Mini
        // Impostor roles with no changes: Morphing, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
    }

    public static void setNameTags()
    {
        // Mafia
        if (CachedPlayer.LocalPlayer.PlayerControl != null && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor)
        {
            foreach (PlayerControl player in CachedPlayer.AllPlayers)
            {
                if (player.cosmetics.nameText.text == "") continue;
                if (player.IsRole(RoleType.Godfather))
                {
                    player.cosmetics.nameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaG")})";
                }
                else if (player.IsRole(RoleType.Mafioso))
                {
                    player.cosmetics.nameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaM")})";
                }
                else if (player.IsRole(RoleType.Janitor))
                {
                    player.cosmetics.nameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaJ")})";
                }
            }
            if (MeetingHud.Instance != null)
            {
                foreach (var voteArea in MeetingHud.Instance.playerStates)
                {
                    var player = Helpers.PlayerById(voteArea.TargetPlayerId);

                    if (player.IsRole(RoleType.Godfather))
                    {
                        voteArea.NameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaG")})";
                    }
                    else if (player.IsRole(RoleType.Mafioso))
                    {
                        voteArea.NameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaM")})";
                    }
                    else if (player.IsRole(RoleType.Janitor))
                    {
                        voteArea.NameText.text = player.Data.PlayerName + $" ({Tr.Get("mafiaJ")})";
                    }
                }
            }
        }

        bool meetingShow = MeetingHud.Instance != null && (MeetingHud.Instance.state is MeetingHud.VoteStates.Voted or MeetingHud.VoteStates.NotVoted or MeetingHud.VoteStates.Discussion);

        // Lovers
        if (CachedPlayer.LocalPlayer.PlayerControl.IsLovers() && CachedPlayer.LocalPlayer.PlayerControl.IsAlive())
        {
            string suffix = Lovers.getIcon(CachedPlayer.LocalPlayer.PlayerControl);
            var lover1 = CachedPlayer.LocalPlayer.PlayerControl;
            var lover2 = CachedPlayer.LocalPlayer.PlayerControl.getPartner();

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

        if (ModMapOptions.GhostsSeeRoles && CachedPlayer.LocalPlayer.PlayerControl.IsDead())
        {
            foreach (var couple in Lovers.couples)
            {
                string suffix = Lovers.getIcon(couple.lover1);
                couple.lover1.cosmetics.nameText.text += suffix;
                couple.lover2.cosmetics.nameText.text += suffix;

                if (meetingShow)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (couple.lover1.PlayerId == player.TargetPlayerId || couple.lover2.PlayerId == player.TargetPlayerId)
                        {
                            player.NameText.text += suffix;
                        }
                    }
                }
            }
        }

        // Hacker and Detective
        if (CachedPlayer.LocalPlayer.PlayerControl != null && ModMapOptions.ShowLighterDarker)
        {
            if (meetingShow)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    var target = Helpers.PlayerById(player.TargetPlayerId);
                    if (target != null) player.NameText.text += $" ({(Helpers.isLighterColor(target.Data.DefaultOutfit.ColorId) ? Tr.Get("detectiveLightLabel") : Tr.Get("detectiveDarkLabel"))})";
                }
            }
        }
    }

    public static void updateImpostorKillButton(HudManager __instance)
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
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mafioso) && !Mafia.Mafioso.canKill)
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

    public static void updateUseButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.UseButton.Hide();
    }

    public static void updateSabotageButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.SabotageButton.Hide();
    }

    public static void updateVentButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.ImpostorVentButton.Hide();
    }

    public static void updateReportButton(HudManager __instance)
    {
        if (MeetingHud.Instance) __instance.ReportButton.Hide();
    }

    public static void StopCooldown(PlayerControl __instance)
    {
        if (CustomOptionHolder.StopCooldownOnFixingElecSabotage.GetBool())
        {
            if (Helpers.isOnElecTask())
            {
                __instance.SetKillTimer(__instance.killTimer + Time.fixedDeltaTime);
            }
        }
    }

    public static void impostorSetTarget()
    {
        if (!CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor || !CachedPlayer.LocalPlayer.PlayerControl.CanMove || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
        { // !isImpostor || !canMove || isDead
            FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(null);
            return;
        }

        PlayerControl target = null;
        if (Spy.spy != null || Sidekick.wasSpy || Jackal.wasSpy)
        {
            if (Spy.ImpostorsCanKillAnyone)
            {
                target = Helpers.SetTarget(false, true);
            }
            else
            {
                var listP = new List<PlayerControl>
                    {
                        Spy.spy
                    };
                if (Sidekick.wasTeamRed) listP.Add(Sidekick.sidekick);
                if (Jackal.wasTeamRed) listP.Add(Jackal.jackal);
                target = Helpers.SetTarget(true, true, listP);
            }
        }
        else
        {
            target = Helpers.SetTarget(true, true);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
    }

    public static void CamouflageAndMorphActions()
    {
        float oldCamouflageTimer = Camouflager.camouflageTimer;
        float oldMorphTimer = Morphing.morphTimer;

        Camouflager.camouflageTimer -= Time.deltaTime;
        Morphing.morphTimer -= Time.deltaTime;

        // Everyone but morphing reset
        if (oldCamouflageTimer > 0f && Camouflager.camouflageTimer <= 0f)
        {
            Camouflager.resetCamouflage();
        }

        // Morphing reset
        if (oldMorphTimer > 0f && Morphing.morphTimer <= 0f)
        {
            Morphing.resetMorph();
        }
    }
}