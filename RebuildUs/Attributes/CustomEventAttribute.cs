namespace RebuildUs.Roles;

internal enum CustomEventType
{
    OnMeetingStart,
    OnMeetingEnd,
    OnIntroEnd,
    FixedUpdate,
    OnKill,
    OnDeath,
    OnFinishShipStatusBegin,
    HandleDisconnect
}

[AttributeUsage(AttributeTargets.Method)]
internal class CustomEventAttribute : Attribute
{
    public CustomEventType EventType { get; }

    public CustomEventAttribute(CustomEventType eventType)
    {
        EventType = eventType;
    }
}
