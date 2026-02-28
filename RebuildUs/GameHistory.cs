using Submerged.KillAnimation.Patches;

namespace RebuildUs;

internal sealed class DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
{
    internal readonly PlayerControl KillerIfExisting = killerIfExisting;
    internal readonly PlayerControl Player = player;
    internal DeathReason DeathReason = deathReason;
    internal DateTime TimeOfDeath = timeOfDeath;
}

internal static class GameHistory
{
    internal static readonly List<(Vector3 pos, bool canMove)> LocalPlayerPositions = [];
    internal static readonly List<DeadPlayer> DeadPlayers = [];
    internal static readonly Dictionary<int, FinalStatus> FinalStatuses = [];

    private static bool _resetToCrewmate;
    private static bool _resetToDead;

    internal static DeadPlayer GetDeadPlayer(byte playerId)
    {
        foreach (var deadPlayer in DeadPlayers)
        {
            if (deadPlayer.Player.PlayerId == playerId)
            {
                return deadPlayer;
            }
        }

        return null;
    }

    internal static void ClearGameHistory()
    {
        LocalPlayerPositions.Clear();
        DeadPlayers.Clear();
        FinalStatuses.Clear();
    }

    internal static void OnMurderPlayerPrefix(PlayerControl killer, PlayerControl target)
    {
        // Allow everyone to murder players
        _resetToCrewmate = !killer.Data.Role.IsImpostor;
        _resetToDead = killer.Data.IsDead;
        killer.Data.Role.TeamType = RoleTeamTypes.Impostor;
        killer.Data.IsDead = false;

        if (Morphing.Exists && target.IsRole(RoleType.Morphing))
        {
            Morphing.ResetMorph();
        }

        target.ResetMorph();
    }

    internal static void OnMurderPlayerPostfix(PlayerControl killer, PlayerControl target)
    {
        StringBuilder sb = new();
        sb.Append(killer.GetNameWithRole());
        sb.Append(" => ");
        sb.Append(target.GetNameWithRole());
        Logger.LogInfo(sb.ToString(), "MurderPlayer");

        // Collect dead player info
        DeadPlayer deadPlayer = new(target, DateTime.UtcNow, DeathReason.Kill, killer);
        DeadPlayers.Add(deadPlayer);

        if (killer.PlayerId == target.PlayerId && SubmergedCompatibility.Loaded && OxygenDeathAnimationPatches.IsOxygenDeath)
        {
            FinalStatuses[target.PlayerId] = FinalStatus.LackOfOxygen;
        }

        // Reset killer to crewmate if resetToCrewmate
        if (_resetToCrewmate)
        {
            killer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        }
        if (_resetToDead)
        {
            killer.Data.IsDead = true;
        }

        // Remove fake tasks when player dies
        if (target.HasFakeTasks())
        {
            target.ClearAllTasks();
        }

        // Seer show flash and add dead player position
        if (Seer.Exists)
        {
            if (PlayerControl.LocalPlayer.IsRole(RoleType.Seer)
                && PlayerControl.LocalPlayer.IsAlive()
                && !target.IsRole(RoleType.Seer)
                && Seer.Mode <= 1)
            {
                Helpers.ShowFlash(new(42f / 255f, 187f / 255f, 245f / 255f));
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
        if (target.IsRole(RoleType.Bait) && Bait.ShowKillFlash && !killer.IsRole(RoleType.Bait) && killer == PlayerControl.LocalPlayer)
        {
            Helpers.ShowFlash(new(42f / 255f, 187f / 255f, 245f / 255f));
        }

        // // impostor promote to last impostor
        if (target.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }

        killer.OnKill(target);
        target.OnDeath(killer);
    }

    internal static void OnExiled(PlayerControl player)
    {
        // Collect dead player info
        DeadPlayer deadPlayer = new(player, DateTime.UtcNow, DeathReason.Exile, null);
        DeadPlayers.Add(deadPlayer);
        FinalStatuses[player.PlayerId] = FinalStatus.Exiled;

        // Remove fake tasks when player dies
        if (player.HasFakeTasks())
        {
            player.ClearAllTasks();
        }

        player.OnDeath(null);

        // impostor promote to last impostor
        if (player.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }
    }
}