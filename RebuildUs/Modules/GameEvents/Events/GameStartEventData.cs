using Iced.Intel;

namespace RebuildUs.Modules.GameEvents.Events;

public class GameStartEventData : GameEventData
{
    public ByteArrayCodeReader MapId { get; set; }
    public int PlayerCount { get; set; }
    public Dictionary<PlayerControl, RoleType> RoleAssignments { get; set; } = [];
}