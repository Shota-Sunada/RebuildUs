namespace RebuildUs.Modules.GameEvents.Events;

public class VentMoveEventData : GameEventData
{
    public PlayerControl Player { get; set; }
    public Vent FromVent { get; set; }
    public Vent ToVent { get; set; }
}