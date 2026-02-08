namespace RebuildUs.Modules.GameEvents.Events;

public class RoleActionEventData : GameEventData
{
    public PlayerControl Player { get; set; }
    public string Action { get; set; }
}
