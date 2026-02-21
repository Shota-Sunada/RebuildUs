namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class SabotageFixEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
    internal string SabotageType { get; set; }
}