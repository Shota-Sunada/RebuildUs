namespace RebuildUs;

public class DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
{
    public PlayerControl Player = player;
    public DateTime TimeOfDeath = timeOfDeath;
    public DeathReason DeathReason = deathReason;
    public PlayerControl KillerIfExisting = killerIfExisting;
}

static class GameHistory
{
    public static List<Tuple<Vector3, bool>> LocalPlayerPositions = [];
    public static List<DeadPlayer> DeadPlayers = [];
    public static Dictionary<int, EFinalStatus> FinalStatuses = [];

    public static DeadPlayer GetDeadPlayer(byte playerId)
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

    public static void ClearGameHistory()
    {
        LocalPlayerPositions.Clear();
        DeadPlayers.Clear();
        FinalStatuses.Clear();
    }

    private static bool ResetToCrewmate = false;
    private static bool ResetToDead = false;

    public static void OnMurderPlayerPrefix(PlayerControl killer, PlayerControl target)
    {
        // Allow everyone to murder players
        ResetToCrewmate = !killer.Data.Role.IsImpostor;
        ResetToDead = killer.Data.IsDead;
        killer.Data.Role.TeamType = RoleTeamTypes.Impostor;
        killer.Data.IsDead = false;

        if (Morphing.Exists && target.IsRole(RoleType.Morphing))
        {
            Morphing.ResetMorph();
        }

        target.ResetMorph();
    }

    public static void OnMurderPlayerPostfix(PlayerControl killer, PlayerControl target)
    {
        var sb = new StringBuilder();
        sb.Append(killer.GetNameWithRole());
        sb.Append(" => ");
        sb.Append(target.GetNameWithRole());
        Logger.LogInfo(sb.ToString(), "MurderPlayer");

        // Collect dead player info
        var deadPlayer = new DeadPlayer(target, DateTime.UtcNow, DeathReason.Kill, killer);
        DeadPlayers.Add(deadPlayer);

        // Reset killer to crewmate if resetToCrewmate
        if (ResetToCrewmate) killer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
        if (ResetToDead) killer.Data.IsDead = true;

        AllPlayers.OnKill(killer, target, deadPlayer);

        killer.OnKill(target);
        target.OnDeath(killer);
    }

    public static void OnExiled(PlayerControl player)
    {
        // Collect dead player info
        var deadPlayer = new DeadPlayer(player, DateTime.UtcNow, DeathReason.Exile, null);
        DeadPlayers.Add(deadPlayer);

        // Remove fake tasks when player dies
        if (player.HasFakeTasks())
        {
            player.ClearAllTasks();
        }

        player.OnDeath(killer: null);

        // impostor promote to last impostor
        if (player.IsTeamImpostor() && AmongUsClient.Instance.AmHost)
        {
            LastImpostor.PromoteToLastImpostor();
        }
    }
}