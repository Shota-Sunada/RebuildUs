namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class RoleActionEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
    internal string Action { get; set; }
}