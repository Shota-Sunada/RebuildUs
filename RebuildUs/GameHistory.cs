using Submerged.KillAnimation.Patches;

namespace RebuildUs;

public sealed class DeadPlayer(PlayerControl player,
                               DateTime timeOfDeath,
                               DeathReason deathReason,
                               PlayerControl killerIfExisting)
{
    internal readonly DeathReason DeathReason = deathReason;
    internal readonly PlayerControl KillerIfExisting = killerIfExisting;
    internal readonly PlayerControl Player = player;
    internal DateTime TimeOfDeath = timeOfDeath;
}

internal static class GameHistory
{
    public static readonly List<Tuple<Vector3, bool>> LOCAL_PLAYER_POSITIONS = [];
    public static readonly List<DeadPlayer> DEAD_PLAYERS = [];
    public static readonly Dictionary<int, FinalStatus> FINAL_STATUSES = [];

    private static bool _resetToCrewmate;
    private static bool _resetToDead;

    public static DeadPlayer GetDeadPlayer(byte playerId)
    {
        foreach (var deadPlayer in DEAD_PLAYERS)
        {
            if (deadPlayer.Player.PlayerId == playerId) return deadPlayer;
        }

        return null;
    }

    public static void ClearGameHistory()
    {
        LOCAL_PLAYER_POSITIONS.Clear();
        DEAD_PLAYERS.Clear();
        FINAL_STATUSES.Clear();
    }

    public static void OnMurderPlayerPrefix(PlayerControl killer, PlayerControl target)
    {
        // Allow everyone to murder players
        _resetToCrewmate = !killer.Data.Role.IsImpostor;
        _resetToDead = killer.Data.IsDead;
        killer.Data.Role.TeamType = RoleTeamTypes.Impostor;
        killer.Data.IsDead = false;

        if (Morphing.Exists && target.IsRole(RoleType.Morphing)) Morphing.ResetMorph();

        target.ResetMorph();
    }

    public static DeadPlayer OnMurderPlayerPostfix(PlayerControl killer, PlayerControl target)
    {
        var sb = new StringBuilder();
        sb.Append(killer.GetNameWithRole());
        sb.Append(" => ");
        sb.Append(target.GetNameWithRole());
        Logger.LogInfo(sb.ToString(), "MurderPlayer");

        // Collect dead player info
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeathReason.Kill, killer);
        DEAD_PLAYERS.Add(deadPlayer);

        if (killer.PlayerId == target.PlayerId
            && SubmergedCompatibility.Loaded
            && OxygenDeathAnimationPatches.IsOxygenDeath)
        {
            FINAL_STATUSES[target.PlayerId] = FinalStatus.LackOfOxygen;
        }

        // Reset killer to crewmate if resetToCrewmate
        if (_resetToCrewmate) killer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (_resetToDead) killer.Data.IsDead = true;

        if (MeetingHud.Instance) DiscordAutoMuteManager.UpdatePlayerMute(target);

        return deadPlayer;
    }

    public static void OnExiled(PlayerControl player)
    {
        // Collect dead player info
        var deadPlayer = new DeadPlayer(player, DateTime.UtcNow, DeathReason.Exile, null);
        DEAD_PLAYERS.Add(deadPlayer);
        FINAL_STATUSES[player.PlayerId] = FinalStatus.Exiled;

        // Remove fake tasks when player dies
        if (player.HasFakeTasks()) player.ClearAllTasks();

        player.OnDeath(null);

        // impostor promote to last impostor
        if (player.IsTeamImpostor() && AmongUsClient.Instance.AmHost) LastImpostor.PromoteToLastImpostor();
    }
}
