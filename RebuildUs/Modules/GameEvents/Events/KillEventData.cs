namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class KillEventData : GameEventData
{
    internal PlayerControl Killer { get; set; }
    internal PlayerControl Victim { get; set; }
}