namespace RebuildUs.Modules.GameEvents.Events;

public class MeetingEndEventData : GameEventData
{
    public MeetingResult Result { get; set; }
    public PlayerControl ExiledPlayer { get; set; }
    public Dictionary<PlayerControl, PlayerControl> Votes { get; set; } = [];
}
