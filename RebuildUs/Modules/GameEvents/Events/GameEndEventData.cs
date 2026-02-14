namespace RebuildUs.Modules.GameEvents.Events;

public class GameEndEventData : GameEventData
{
    public string WinningTeam { get; set; }
    public GameOverReason Reason { get; set; }
}