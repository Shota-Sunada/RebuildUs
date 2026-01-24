namespace RebuildUs.Modules.GameEvents.Events;

public class DisconnectEventData : GameEventData
{
    public PlayerControl Player { get; set; }
}