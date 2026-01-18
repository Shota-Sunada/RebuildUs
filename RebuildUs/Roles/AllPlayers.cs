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

        // Seer show flash and add dead player position
        if (Seer.Exists)
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleType.Seer) && !PlayerControl.LocalPlayer.Data.IsDead && !target.IsRole(RoleType.Seer) && Seer.Mode <= 1)
            {
                Helpers.ShowFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
            }
            Seer.DeadBodyPositions?.Add(target.transform.position);
        }

        // // Tracker store body positions
        Tracker.DeadBodyPositions?.Add(target.transform.position);

        // Medium add body
        if (Medium.DeadBodies != null)
        {
            Medium.FeatureDeadBodies.Add(new(deadPlayer, target.transform.position));
        }

        // // Mini set adapted kill cooldown
        // if (PlayerControl.LocalPlayer.hasModifier(ModifierType.Mini) && PlayerControl.LocalPlayer.Data.Role.IsImpostor && PlayerControl.LocalPlayer == __instance)
        // {
        //     var multiplier = Mini.isGrownUp(PlayerControl.LocalPlayer) ? 0.66f : 2f;
        //     PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        // }

        // Show flash on bait kill to the killer if enabled
        if (target.IsRole(RoleType.Bait) && Bait.ShowKillFlash && !__instance.IsRole(RoleType.Bait) && __instance == PlayerControl.LocalPlayer)
        {
            Helpers.ShowFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }
    }
}