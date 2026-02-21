namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class SabotageStartEventData : GameEventData
{
    internal string SabotageType { get; set; }
}