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
internal class CustomEventAttribute(CustomEventType eventType) : Attribute
{
    public CustomEventType EventType { get; } = eventType;
}