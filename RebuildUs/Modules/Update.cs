using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Impostor;

namespace RebuildUs.Patches;

public static class Update
{
    public static void resetNameTagsAndColors()
    {
        var playersById = Helpers.allPlayersById();

        foreach (PlayerControl player in CachedPlayer.AllPlayers)
        {
            player.cosmetics.nameText.text = Helpers.hidePlayerName(CachedPlayer.LocalPlayer.PlayerControl, player) ? "" : player.CurrentOutfit.PlayerName;
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
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Jester.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Mayor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Mayor.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Engineer))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Engineer.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Sheriff))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Sheriff.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Lighter))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Lighter.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Detective))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Detective.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.TimeMaster))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, TimeMaster.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Medic.color);
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
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Seer.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Hacker))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Hacker.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Tracker))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Tracker.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Snitch))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Snitch.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Jackal))
        {
            // Jackal can see his sidekick
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Jackal.color);
            if (Sidekick.sidekick != null)
            {
                setPlayerNameColor(Sidekick.sidekick, Jackal.color);
            }
            if (Jackal.fakeSidekick != null)
            {
                setPlayerNameColor(Jackal.fakeSidekick, Jackal.color);
            }
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Spy))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Spy.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.SecurityGuard))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, SecurityGuard.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Arsonist))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Arsonist.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.NiceGuesser))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Guesser.color);
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
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Vulture.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medium))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Medium.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Lawyer))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Lawyer.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Pursuer))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Pursuer.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.PlagueDoctor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, PlagueDoctor.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Fox))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Fox.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Immoralist))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Immoralist.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.FortuneTeller) && (FortuneTeller.isCompletedNumTasks(CachedPlayer.LocalPlayer.PlayerControl) || CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, FortuneTeller.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Akujo))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Akujo.color);
        }
        else if (PlayerControl.LocalPlayer.IsRole(RoleType.Sherlock))
        {
            setPlayerNameColor(PlayerControl.LocalPlayer, Sherlock.color);
        }
        else if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Cupid) && CachedPlayer.LocalPlayer.PlayerControl.isAlive())
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Cupid.color);
            var cupid = Cupid.allRoles.FirstOrDefault(x => x.player == PlayerControl.LocalPlayer) as Cupid;
            bool meetingShow = MeetingHud.Instance != null &&
                (MeetingHud.Instance.state == MeetingHud.VoteStates.Voted ||
                MeetingHud.Instance.state == MeetingHud.VoteStates.NotVoted ||
                MeetingHud.Instance.state == MeetingHud.VoteStates.Discussion);
            string suffix = Helpers.cs(Cupid.color, " ♥");
            foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
            {
                if (p == cupid.lovers1 || p == cupid.lovers2)
                {
                    p.cosmetics.nameText.text += suffix;
                    if (meetingShow)
                    {
                        foreach (PlayerVoteArea pva in MeetingHud.Instance.playerStates)
                        {
                            if (p.PlayerId == pva.TargetPlayerId)
                            {
                                pva.NameText.text += suffix;
                            }
                        }
                    }
                }
            }
        }

        if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Madmate))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.color);

            if (Madmate.knowsImpostors(CachedPlayer.LocalPlayer.PlayerControl))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.wasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.wasTeamRed))
                    {
                        setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.CreatedMadmate))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Madmate.color);

            if (CreatedMadmate.knowsImpostors(CachedPlayer.LocalPlayer.PlayerControl))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor() || p.IsRole(RoleType.Spy) || (p.IsRole(RoleType.Jackal) && Jackal.wasTeamRed) || (p.IsRole(RoleType.Sidekick) && Sidekick.wasTeamRed))
                    {
                        setPlayerNameColor(p, Palette.ImpostorRed);
                    }
                }
            }
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.LastImpostor))
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, LastImpostor.color);
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Munou) && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
        {
            setPlayerNameColor(CachedPlayer.LocalPlayer.PlayerControl, Munou.color);
        }

        else if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.AntiTeleport) && CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead)
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
            setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
            if (Jackal.jackal != null)
            {
                setPlayerNameColor(Jackal.jackal, Jackal.color);
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

        if (Immoralist.exists && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Fox))
        {
            foreach (var immoralist in Immoralist.allPlayers)
            {
                setPlayerNameColor(immoralist, Immoralist.color);
            }
        }

        if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Immoralist))
        {
            foreach (var fox in Fox.allPlayers)
            {
                setPlayerNameColor(fox, Fox.color);
            }
        }

        var player = CachedPlayer.LocalPlayer.PlayerControl;
        bool impostorFlag = player.IsRole(RoleType.SchrodingersCat) || player.IsTeamImpostor();
        bool jackalFlag = player.IsRole(RoleType.SchrodingersCat) || player.IsRole(RoleType.Jackal) || player.IsRole(RoleType.Sidekick);
        bool jekyllAndHydeFlag = player.IsRole(RoleType.SchrodingersCat) || player.IsRole(RoleType.JekyllAndHyde);
        bool moriartyFlag = player.IsRole(RoleType.SchrodingersCat) || player.IsRole(RoleType.Moriarty);
        if (!SchrodingersCat.hasTeam() && SchrodingersCat.hideRole && CachedPlayer.LocalPlayer.PlayerControl.isAlive())
        {
            // 何もしない
        }
        else if (SchrodingersCat.team == SchrodingersCat.Team.Crew)
        {
            foreach (var p in SchrodingersCat.allPlayers)
            {
                setPlayerNameColor(p, Color.white);
            }
        }
        else if (SchrodingersCat.team == SchrodingersCat.Team.Impostor && impostorFlag)
        {
            foreach (var p in SchrodingersCat.allPlayers)
            {
                setPlayerNameColor(p, Palette.ImpostorRed);
            }
            if (player.IsRole(RoleType.SchrodingersCat))
            {
                foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                {
                    if (p.IsTeamImpostor()) setPlayerNameColor(p, Palette.ImpostorRed);
                }
            }
        }
        else if (SchrodingersCat.team == SchrodingersCat.Team.Jackal && jackalFlag)
        {
            foreach (var p in SchrodingersCat.allPlayers)
            {
                setPlayerNameColor(p, Jackal.color);
            }
            if (player.IsRole(RoleType.SchrodingersCat))
            {
                setPlayerNameColor(Jackal.jackal, Jackal.color);
                if (Sidekick.sidekick != null) setPlayerNameColor(Sidekick.sidekick, Sidekick.color);
            }
        }
        else if (SchrodingersCat.team == SchrodingersCat.Team.JekyllAndHyde && jekyllAndHydeFlag)
        {
            foreach (var p in SchrodingersCat.allPlayers)
            {
                setPlayerNameColor(p, JekyllAndHyde.color);
            }
            if (player.IsRole(RoleType.SchrodingersCat))
            {
                foreach (var p in JekyllAndHyde.allPlayers)
                {
                    setPlayerNameColor(p, JekyllAndHyde.color);
                }
            }
        }
        else if (SchrodingersCat.team == SchrodingersCat.Team.Moriarty && moriartyFlag)
        {
            foreach (var p in SchrodingersCat.allPlayers)
            {
                setPlayerNameColor(p, Moriarty.color);
            }
            if (player.IsRole(RoleType.SchrodingersCat))
            {
                foreach (var p in Moriarty.allPlayers)
                {
                    setPlayerNameColor(p, Moriarty.color);
                }
            }
        }
        else if (player.IsRole(RoleType.SchrodingersCat))
        {
            setPlayerNameColor(player, SchrodingersCat.color);
        }

        // Crewmate roles with no changes: Mini
        // Impostor roles with no changes: Morphling, Camouflager, Vampire, Godfather, Eraser, Janitor, Cleaner, Warlock, BountyHunter,  Witch and Mafioso
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
        if (CachedPlayer.LocalPlayer.PlayerControl.isLovers() && CachedPlayer.LocalPlayer.PlayerControl.isAlive())
        {
            string suffix = Lovers.getIcon(CachedPlayer.LocalPlayer.PlayerControl);
            if (Cupid.isCupidLovers(CachedPlayer.LocalPlayer))
            {
                suffix = Helpers.cs(Cupid.color, " ♥");
            }
            var lover1 = CachedPlayer.LocalPlayer.PlayerControl;
            var lover2 = CachedPlayer.LocalPlayer.PlayerControl.getPartner();

            lover1.cosmetics.nameText.text += suffix;
            if (!Helpers.hidePlayerName(lover2))
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

        // Akujo
        if (CachedPlayer.LocalPlayer.PlayerControl.isAlive() && CachedPlayer.LocalPlayer.PlayerControl.isAkujoLover())
        {
            foreach (var akujo in Akujo.players)
            {
                string suffix = Helpers.cs(akujo.iconColor, " ♥");
                if (CachedPlayer.LocalPlayer.PlayerControl == akujo.player)
                {
                    CachedPlayer.LocalPlayer.PlayerControl.cosmetics.nameText.text += suffix;
                    if (akujo.honmei != null && !Helpers.hidePlayerName(akujo.honmei.player))
                        akujo.honmei.player.cosmetics.nameText.text += suffix;

                    foreach (var keep in akujo.keeps)
                    {
                        if (!Helpers.hidePlayerName(keep.player))
                            keep.player.cosmetics.nameText.text += suffix;
                    }

                    if (Helpers.ShowMeetingText)
                    {
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        {
                            if (CachedPlayer.LocalPlayer.PlayerControl.PlayerId == player.TargetPlayerId ||
                                akujo.honmei?.player?.PlayerId == player.TargetPlayerId ||
                                akujo.keeps.Any(x => x.player.PlayerId == player.TargetPlayerId))
                            {
                                player.NameText.text += suffix;
                            }
                        }
                    }
                }

                else if (CachedPlayer.LocalPlayer.PlayerControl == akujo.honmei?.player || akujo.keeps.Any(x => CachedPlayer.LocalPlayer.PlayerControl == x?.player))
                {
                    CachedPlayer.LocalPlayer.PlayerControl.cosmetics.nameText.text += suffix;
                    if (!Helpers.hidePlayerName(akujo.player))
                        akujo.player.cosmetics.nameText.text += suffix;

                    if (Helpers.ShowMeetingText)
                    {
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                        {
                            if (CachedPlayer.LocalPlayer.PlayerControl.PlayerId == player.TargetPlayerId || akujo.player.PlayerId == player.TargetPlayerId)
                                player.NameText.text += suffix;
                        }
                    }
                }
            }
        }

        if (ModMapOptions.ghostsSeeRoles && CachedPlayer.LocalPlayer.PlayerControl.isDead())
        {
            foreach (var couple in Lovers.couples)
            {
                string suffix = Lovers.getIcon(couple.lover1);
                if (Cupid.isCupidLovers(couple.lover1))
                {
                    suffix = Helpers.cs(Cupid.color, " ♥");
                }
                couple.lover1.cosmetics.nameText.text += suffix;
                couple.lover2.cosmetics.nameText.text += suffix;

                if (meetingShow)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (couple.lover1.PlayerId == player.TargetPlayerId || couple.lover2.PlayerId == player.TargetPlayerId)
                            player.NameText.text += suffix;
                    }
                }
            }
            foreach (var akujo in Akujo.players)
            {
                string suffix = Helpers.cs(akujo.iconColor, " ♥");
                akujo.player.cosmetics.nameText.text += suffix;
                if (akujo.honmei != null)
                    akujo.honmei.player.cosmetics.nameText.text += suffix;
                foreach (var keep in akujo.keeps)
                    keep.player.cosmetics.nameText.text += suffix;

                if (Helpers.ShowMeetingText)
                {
                    foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                    {
                        if (akujo.player.PlayerId == player.TargetPlayerId ||
                            akujo.honmei?.player?.PlayerId == player.TargetPlayerId ||
                            akujo.keeps.Any(x => x.player.PlayerId == player.TargetPlayerId))
                        {
                            player.NameText.text += suffix;
                        }
                    }
                }
            }
        }

        // Lawyer
        bool localIsLawyer = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.lawyer == CachedPlayer.LocalPlayer.PlayerControl;
        bool localIsKnowingTarget = Lawyer.lawyer != null && Lawyer.target != null && Lawyer.targetKnows && Lawyer.target == CachedPlayer.LocalPlayer.PlayerControl;
        if (localIsLawyer || (localIsKnowingTarget && !Lawyer.lawyer.Data.IsDead))
        {
            string suffix = Helpers.cs(Lawyer.color, " §");
            if (!Helpers.hidePlayerName(Lawyer.target))
                Lawyer.target.cosmetics.nameText.text += suffix;

            if (meetingShow)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    if (player.TargetPlayerId == Lawyer.target.PlayerId)
                        player.NameText.text += suffix;
                }
            }
        }

        // Hacker and Detective
        if (CachedPlayer.LocalPlayer.PlayerControl != null && ModMapOptions.showLighterDarker)
        {
            if (meetingShow)
            {
                foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                {
                    var target = Helpers.playerById(player.TargetPlayerId);
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

    public static PlayerControl setTarget(bool onlyCrewmates = false, bool targetPlayersInVents = false, List<PlayerControl> untargetablePlayers = null, PlayerControl targetingPlayer = null, int killDistance = 3)
    {
        PlayerControl result = null;
        int kd = killDistance == 3 ? PlayerControl.GameOptions.KillDistance : killDistance;
        float num = GameOptionsData.KillDistances[Mathf.Clamp(kd, 0, 2)];
        if (!MapUtilities.CachedShipStatus) return result;
        if (targetingPlayer == null) targetingPlayer = PlayerControl.LocalPlayer;
        if ((targetingPlayer.Data.IsDead && !targetingPlayer.IsRole(RoleType.Puppeteer)) || targetingPlayer.inVent) return result;
        if (targetingPlayer.IsGM()) return result;

        if (untargetablePlayers == null)
        {
            untargetablePlayers = new List<PlayerControl>();
        }

        // GM is untargetable by anything
        if (GM.gm != null)
        {
            untargetablePlayers.Add(GM.gm);
        }

        // Can't target stealthed ninjas if setting on
        if (!Ninja.canBeTargeted)
        {
            foreach (Ninja n in Ninja.players)
            {
                if (n.stealthed) untargetablePlayers.Add(n.player);
            }
        }

        Vector2 truePosition = targetingPlayer.GetTruePosition();
        foreach (var playerInfo in GameData.Instance.AllPlayers)
        {
            if (!playerInfo.Disconnected && playerInfo.PlayerId != targetingPlayer.PlayerId && !playerInfo.IsDead && (!onlyCrewmates || !playerInfo.Role.IsImpostor))
            {
                PlayerControl @object = playerInfo.Object;
                if (untargetablePlayers.Any(x => x == @object))
                {
                    // if that player is not targetable: skip check
                    continue;
                }

                if (@object && (!@object.inVent || targetPlayersInVents))
                {
                    Vector2 vector = @object.GetTruePosition() - truePosition;
                    float magnitude = vector.magnitude;
                    if (magnitude <= num && !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
                    {
                        result = @object;
                        num = magnitude;
                    }
                }
            }
        }
        return result;
    }

    public static void setPlayerOutline(PlayerControl target, Color color)
    {
        if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) return;

        target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
        target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", color);
    }

    public static void setBasePlayerOutlines()
    {
        foreach (PlayerControl target in CachedPlayer.AllPlayers)
        {
            if (target == null || target.cosmetics?.currentBodySprite?.BodySprite == null) continue;

            bool isMorphedMorphling = target == Morphling.morphling && Morphling.morphTarget != null && Morphling.morphTimer > 0f;
            bool hasVisibleShield = false;
            foreach (var medic in Medic.Players)
            {
                if (Camouflager.camouflageTimer <= 0f && medic.shielded != null && ((target == medic.shielded && !isMorphedMorphling) || (isMorphedMorphling && Morphling.morphTarget == medic.shielded)))
                {
                    hasVisibleShield = Medic.showShielded == 0 // Everyone
                    || (Medic.showShielded == 1 && (CachedPlayer.LocalPlayer.PlayerControl == medic.shielded || CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic))) // Shielded + Medic
                    || (Medic.showShielded == 2 && CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Medic)); // Medic only
                }
            }

            if (hasVisibleShield)
            {
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 1f);
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetColor("_OutlineColor", Medic.shieldedColor);
            }
            else
            {
                target.cosmetics?.currentBodySprite?.BodySprite.material.SetFloat("_Outline", 0f);
            }
        }
    }

    public static void updatePlayerInfo()
    {
        bool commsActive = false;
        foreach (PlayerTask t in CachedPlayer.LocalPlayer.PlayerControl.myTasks)
        {
            if (t.TaskType == TaskTypes.FixComms)
            {
                commsActive = true;
                break;
            }
        }

        var canSeeEverything = CachedPlayer.LocalPlayer.PlayerControl.isDead() || CachedPlayer.LocalPlayer.PlayerControl.IsGM();
        foreach (PlayerControl p in CachedPlayer.AllPlayers)
        {
            if (p == null) continue;

            bool isAkujo = Akujo.isPartner(CachedPlayer.LocalPlayer.PlayerControl, p);

            var canSeeInfo =
                canSeeEverything || isAkujo ||
                p == CachedPlayer.LocalPlayer.PlayerControl || p.IsGM() ||
                (Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target);

            if (canSeeInfo)
            {
                Transform playerInfoTransform = p.cosmetics.nameText.transform.parent.FindChild("Info");
                TMPro.TextMeshPro playerInfo = playerInfoTransform != null ? playerInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (playerInfo == null)
                {
                    playerInfo = UnityEngine.Object.Instantiate(p.cosmetics.nameText, p.cosmetics.nameText.transform.parent);
                    playerInfo.fontSize *= 0.75f;
                    playerInfo.gameObject.name = "Info";
                }

                // Set the position every time bc it sometimes ends up in the wrong place due to camoflauge
                playerInfo.transform.localPosition = p.cosmetics.nameText.transform.localPosition + Vector3.up * 0.5f;

                PlayerVoteArea playerVoteArea = MeetingHud.Instance?.playerStates?.FirstOrDefault(x => x.TargetPlayerId == p.PlayerId);
                Transform meetingInfoTransform = playerVoteArea != null ? playerVoteArea.NameText.transform.parent.FindChild("Info") : null;
                TMPro.TextMeshPro meetingInfo = meetingInfoTransform != null ? meetingInfoTransform.GetComponent<TMPro.TextMeshPro>() : null;
                if (meetingInfo == null && playerVoteArea != null)
                {
                    meetingInfo = UnityEngine.Object.Instantiate(playerVoteArea.NameText, playerVoteArea.NameText.transform.parent);
                    meetingInfo.transform.localPosition += Vector3.down * 0.10f;
                    meetingInfo.fontSize *= 0.60f;
                    meetingInfo.gameObject.name = "Info";
                }

                // Set player name higher to align in middle
                if (meetingInfo != null && playerVoteArea != null)
                {
                    var playerName = playerVoteArea.NameText;
                    playerName.transform.localPosition = new Vector3(0.3384f, 0.0311f + 0.0683f, -0.1f);
                }

                var (tasksCompleted, tasksTotal) = TasksHandler.taskInfo(p.Data);
                string roleNames = RoleInfo.GetRolesString(p, true, new RoleType[] { RoleType.Lovers });
                string roleNamesFull = RoleInfo.GetRolesString(p, true, new RoleType[] { RoleType.Lovers }, true);


                var completedStr = commsActive ? "?" : tasksCompleted.ToString();
                string taskInfo = tasksTotal > 0 ? $"<color=#FAD934FF>({completedStr}/{tasksTotal})</color>" : "";

                string playerInfoText = "";
                string meetingInfoText = "";
                if (p == CachedPlayer.LocalPlayer.PlayerControl)
                {
                    playerInfoText = $"{roleNames}";
                    if (DestroyableSingleton<TaskPanelBehaviour>.InstanceExists)
                    {
                        TMPro.TextMeshPro tabText = FastDestroyableSingleton<TaskPanelBehaviour>.Instance.tab.transform.FindChild("TabText_TMP").GetComponent<TMPro.TextMeshPro>();
                        tabText.SetText($"{TranslationController.Instance.GetString(StringNames.Tasks)} {taskInfo}");
                    }
                    meetingInfoText = $"{roleNames} {taskInfo}".Trim();
                }
                else if (CachedPlayer.LocalPlayer.PlayerControl.isAlive() && isAkujo)
                {
                    if (Akujo.knowsRoles)
                    {
                        playerInfoText = roleNamesFull;
                        meetingInfoText = roleNamesFull;
                    }
                    else if (p.hasModifier(ModifierType.AkujoHonmei))
                    {
                        playerInfoText = Helpers.cs(Akujo.color, Tr.Get("akujoHonmei"));
                    }
                    else if (p.hasModifier(ModifierType.AkujoKeep))
                    {
                        playerInfoText = Helpers.cs(Akujo.color, Tr.Get("akujoKeep"));
                    }
                }
                else if (ModMapOptions.GhostsSeeRoles && ModMapOptions.ghostsSeeTasks)
                {
                    playerInfoText = $"{roleNames} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (ModMapOptions.ghostsSeeTasks)
                {
                    playerInfoText = $"{taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }
                else if (ModMapOptions.ghostsSeeRoles || (Lawyer.lawyerKnowsRole && CachedPlayer.LocalPlayer.PlayerControl == Lawyer.lawyer && p == Lawyer.target))
                {
                    playerInfoText = $"{roleNames}";
                    meetingInfoText = playerInfoText;
                }
                else if (p.IsGM() || CachedPlayer.LocalPlayer.PlayerControl.IsGM())
                {
                    playerInfoText = $"{roleNames} {taskInfo}".Trim();
                    meetingInfoText = playerInfoText;
                }

                playerInfo.text = playerInfoText;
                playerInfo.gameObject.SetActive(p.Visible && !Helpers.hidePlayerName(p));
                if (meetingInfo != null) meetingInfo.text = MeetingHud.Instance.state == MeetingHud.VoteStates.Results ? "" : meetingInfoText;
            }
        }
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
            if (Spy.impostorsCanKillAnyone)
            {
                target = setTarget(false, true);
            }
            else
            {
                var listp = new List<PlayerControl>
                    {
                        Spy.spy
                    };
                if (Sidekick.wasTeamRed) listp.Add(Sidekick.sidekick);
                if (Jackal.wasTeamRed) listp.Add(Jackal.jackal);
                target = setTarget(true, true, listp);
            }
        }
        else
        {
            target = setTarget(true, true);
        }

        FastDestroyableSingleton<HudManager>.Instance.KillButton.SetTarget(target); // Includes setPlayerOutline(target, Palette.ImpstorRed);
    }

    public static void setPetVisibility()
    {
        bool localalive = PlayerControl.LocalPlayer.Data.IsDead;
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            bool playeralive = !player.Data.IsDead;
            player.cosmetics.SetPetVisible((localalive && playeralive) || !localalive);
        }
    }
}