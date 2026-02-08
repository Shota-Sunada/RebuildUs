namespace RebuildUs.Modules.GameEvents.Events;

public class KillEventData : GameEventData
{
    public PlayerControl Killer { get; set; }
    public PlayerControl Victim { get; set; }
}
