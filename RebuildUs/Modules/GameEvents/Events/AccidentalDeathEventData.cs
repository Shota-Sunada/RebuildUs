namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class AccidentalDeathEventData : GameEventData
{
    internal PlayerControl Victim { get; set; }
    internal string Cause { get; set; }
}