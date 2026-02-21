using Submerged.KillAnimation.Patches;

namespace RebuildUs;

internal sealed class DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
{
    internal DeathReason DeathReason = deathReason;
    internal PlayerControl KillerIfExisting = killerIfExisting;
    internal PlayerControl Player = player;
    internal DateTime TimeOfDeath = timeOfDeath;
}

internal static class GameHistory
{
    internal static List<Tuple<Vector3, bool>> LocalPlayerPositions = [];
    internal static List<DeadPlayer> DeadPlayers = [];
    internal static Dictionary<int, FinalStatus> FinalStatuses = [];

    private static bool _resetToCrewmate;
    private static bool _resetToDead;

    internal static DeadPlayer GetDeadPlayer(byte playerId)
    {
        foreach (DeadPlayer deadPlayer in DeadPlayers)
        {
            if (deadPlayer.Player.PlayerId == playerId)
                return deadPlayer;
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

        if (Morphing.Exists && target.IsRole(RoleType.Morphing)) Morphing.ResetMorph();

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

        if (killer.PlayerId == target.PlayerId && SubmergedCompatibility.Loaded && OxygenDeathAnimationPatches.IsOxygenDeath) FinalStatuses[target.PlayerId] = FinalStatus.LackOfOxygen;

        // Reset killer to crewmate if resetToCrewmate
        if (_resetToCrewmate) killer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (_resetToDead) killer.Data.IsDead = true;

        AllPlayers.OnKill(killer, target, deadPlayer);

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
        if (player.HasFakeTasks()) player.ClearAllTasks();

        player.OnDeath(null);

        // impostor promote to last impostor
        if (player.IsTeamImpostor() && AmongUsClient.Instance.AmHost) LastImpostor.PromoteToLastImpostor();
    }
}