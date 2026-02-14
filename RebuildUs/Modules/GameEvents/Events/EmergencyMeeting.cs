namespace RebuildUs.Modules.GameEvents.Events;

public class EmergencyMeetingEventData : GameEventData
{
    public PlayerControl Caller { get; set; }
}