namespace RebuildUs;

public class DeadPlayer
{
    public PlayerControl Player;
    public DateTime TimeOfDeath;
    public DeathReason DeathReason;
    public PlayerControl KillerIfExisting;

    public DeadPlayer(PlayerControl player, DateTime timeOfDeath, DeathReason deathReason, PlayerControl killerIfExisting)
    {
        this.Player = player;
        this.TimeOfDeath = timeOfDeath;
        this.DeathReason = deathReason;
        this.KillerIfExisting = killerIfExisting;
    }
}

static class GameHistory
{
    public static List<Tuple<Vector3, bool>> LocalPlayerPositions = [];
    public static List<DeadPlayer> DeadPlayers = [];
    public static Dictionary<int, EFinalStatus> FinalStatuses = [];

    public static void ClearGameHistory()
    {
        LocalPlayerPositions = [];
        DeadPlayers = [];
        FinalStatuses = [];
    }
}