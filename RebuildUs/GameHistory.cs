namespace RebuildUs;

public class DeadPlayer
{
    public PlayerControl player;
    public DateTime timeOfDeath;
    public DeathReason deathReason;
    public PlayerControl killerIfExisting;

    public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
    {
        this.player = player;
        this.timeOfDeath = timeOfDeath;
        this.deathReason = deathReason;
        this.killerIfExisting = killerIfExisting;
    }
}

static class GameHistory
{
    public static List<Tuple<Vector3, bool>> localPlayerPositions = [];
    public static List<DeadPlayer> deadPlayers = [];
    public static Dictionary<int, EFinalStatus> finalStatuses = [];

    public static void ClearGameHistory()
    {
        localPlayerPositions = [];
        deadPlayers = [];
        finalStatuses = [];
    }
}
