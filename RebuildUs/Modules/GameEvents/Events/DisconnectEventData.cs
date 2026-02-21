namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class DisconnectEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
}