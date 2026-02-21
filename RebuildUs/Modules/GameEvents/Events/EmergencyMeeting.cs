namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class EmergencyMeetingEventData : GameEventData
{
    internal PlayerControl Caller { get; set; }
}