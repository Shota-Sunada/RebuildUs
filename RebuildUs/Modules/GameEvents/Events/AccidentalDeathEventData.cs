namespace RebuildUs.Modules.GameEvents.Events;

public class AccidentalDeathEventData : GameEventData
{
    public PlayerControl Victim { get; set; }
    public string Cause { get; set; }
}
