namespace RebuildUs.Modules.GameEvents.Events;

public class PlayerStateChangeEventData : GameEventData
{
    public PlayerControl Player { get; set; }
    public FinalStatus FinalStatus { get; set; }
}