namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class VentMoveEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
    internal Vent FromVent { get; set; }
    internal Vent ToVent { get; set; }
}