namespace RebuildUs.Modules.GameEvents.Events;

public class BodyReportEventData : GameEventData
{
    public PlayerControl Reporter { get; set; }
    public PlayerControl DeadPlayer { get; set; }
}
