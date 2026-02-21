namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class MeetingEndEventData : GameEventData
{
    internal MeetingResult Result { get; set; }
    internal PlayerControl ExiledPlayer { get; set; }
    internal Dictionary<PlayerControl, PlayerControl> Votes { get; set; } = [];
}