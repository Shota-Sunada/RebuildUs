namespace RebuildUs.Modules.GameEvents.Events;

public class SabotageFixEventData : GameEventData
{
    public PlayerControl Player { get; set; }
    public string SabotageType { get; set; }
}
