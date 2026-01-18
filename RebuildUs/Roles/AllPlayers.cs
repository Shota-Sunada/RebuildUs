using RebuildUs.Roles.Crewmate;
using RebuildUs.Roles.Modifier;

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
            if (CachedPlayer.LocalPlayer.PlayerControl.IsRole(RoleType.Seer) && !CachedPlayer.LocalPlayer.PlayerControl.Data.IsDead && !target.IsRole(RoleType.Seer) && Seer.mode <= 1)
            {
                Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
            }
            Seer.deadBodyPositions?.Add(target.transform.position);
        }

        // // Tracker store body positions
        Tracker.deadBodyPositions?.Add(target.transform.position);

        // Medium add body
        if (Medium.deadBodies != null)
        {
            Medium.featureDeadBodies.Add(new(deadPlayer, target.transform.position));
        }

        // // Mini set adapted kill cooldown
        // if (CachedPlayer.LocalPlayer.PlayerControl.hasModifier(ModifierType.Mini) && CachedPlayer.LocalPlayer.PlayerControl.Data.Role.IsImpostor && CachedPlayer.LocalPlayer.PlayerControl == __instance)
        // {
        //     var multiplier = Mini.isGrownUp(CachedPlayer.LocalPlayer.PlayerControl) ? 0.66f : 2f;
        //     CachedPlayer.LocalPlayer.PlayerControl.SetKillTimer(PlayerControl.GameOptions.KillCooldown * multiplier);
        // }

        // Show flash on bait kill to the killer if enabled
        if (target.IsRole(RoleType.Bait) && Bait.showKillFlash && !__instance.IsRole(RoleType.Bait) && __instance == CachedPlayer.LocalPlayer.PlayerControl)
        {
            Helpers.showFlash(new Color(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.promoteToLastImpostor();
        }
    }
}
