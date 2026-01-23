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

    public static void ClearGameHistory()
    {
        LocalPlayerPositions.Clear();
        DeadPlayers.Clear();
        FinalStatuses.Clear();
    }
}