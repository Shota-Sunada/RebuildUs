using RebuildUs.Roles.Crewmate;

namespace RebuildUs.Roles;

public static class AllPlayers
{
    public static void OnKill(PlayerControl __instance, PlayerControl target, DeadPlayer deadPlayer)
    {
        // Remove fake tasks when player dies
        if (target.HasFakeTasks())
        {
            target.ClearAllTasks();
        }

        // Sidekick promotion trigger on murder
        if (Sidekick.promotesToJackal && Sidekick.sidekick != null && !Sidekick.sidekick.Data.IsDead && target == Jackal.jackal && Jackal.jackal == CachedPlayer.LocalPlayer.PlayerControl)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.SidekickPromotes, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.sidekickPromotes();
        }

        // Pursuer promotion trigger on murder (the host sends the call such that everyone receives the update before a possible game End)
        if (target == Lawyer.target && AmongUsClient.Instance.AmHost)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(CachedPlayer.LocalPlayer.PlayerControl.NetId, (byte)CustomRPC.LawyerPromotesToPursuer, Hazel.SendOption.Reliable, -1);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            RPCProcedure.lawyerPromotesToPursuer();
        }

        // Cleaner Button Sync
        if (Cleaner.cleaner != null && CachedPlayer.LocalPlayer.PlayerControl == Cleaner.cleaner && __instance == Cleaner.cleaner && HudManagerStartPatch.cleanerCleanButton != null)
            HudManagerStartPatch.cleanerCleanButton.Timer = Cleaner.cleaner.killTimer;

        // Witch Button Sync
        if (Witch.triggerBothCooldowns && Witch.witch != null && CachedPlayer.LocalPlayer.PlayerControl == Witch.witch && __instance == Witch.witch && HudManagerStartPatch.witchSpellButton != null)
            HudManagerStartPatch.witchSpellButton.Timer = HudManagerStartPatch.witchSpellButton.MaxTimer;

        // Warlock Button Sync
        if (Warlock.warlock != null && CachedPlayer.LocalPlayer.PlayerControl == Warlock.warlock && __instance == Warlock.warlock && HudManagerStartPatch.warlockCurseButton != null)
        {
            if (Warlock.warlock.killTimer > HudManagerStartPatch.warlockCurseButton.Timer)
            {
                HudManagerStartPatch.warlockCurseButton.Timer = Warlock.warlock.killTimer;
            }
        }

        // Assassin Button Sync
        if (Assassin.assassin != null && CachedPlayer.LocalPlayer.PlayerControl == Assassin.assassin && __instance == Assassin.assassin && HudManagerStartPatch.assassinButton != null)
            HudManagerStartPatch.assassinButton.Timer = HudManagerStartPatch.assassinButton.MaxTimer;

        // Seer show flash and add dead player position
        if (Seer.Exists)
        {
            if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Seer) && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && !target.IsRole(RoleType.Seer) && Seer.mode <= 1)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
            }
            Seer.deadBodyPositions?.Add(target.transform.position);
        }

        // Tracker store body positions
        if (Tracker.deadBodyPositions != null) Tracker.deadBodyPositions.Add(target.transform.position);

        // Medium add body
        if (Medium.deadBodies != null)
        {
            Medium.featureDeadBodies.Add(new(deadPlayer, target.transform.position));
        }

        // Mini set adapted kill cooldown
        if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        {
            var multiplier = Mini.isGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f;
            CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        }

        // Update arsonist status
        Arsonist.updateStatus();

        // Show flash on bait kill to the killer if enabled
        if (Bait.bait != null && target == Bait.bait && Bait.showKillFlash && __instance != Bait.bait && __instance == CachedPlayer.LocalPlayer.PlayerControl)
        {
            Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }
    }
}