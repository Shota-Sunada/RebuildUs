namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class BodyReportEventData : GameEventData
{
    internal PlayerControl Reporter { get; set; }
    internal PlayerControl DeadPlayer { get; set; }
}