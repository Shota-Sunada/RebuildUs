namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class PlayerStateChangeEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
    internal FinalStatus FinalStatus { get; set; }
}