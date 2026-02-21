using Iced.Intel;

namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class GameStartEventData : GameEventData
{
    internal ByteArrayCodeReader MapId { get; set; }
    internal int PlayerCount { get; set; }
    internal Dictionary<PlayerControl, RoleType> RoleAssignments { get; set; } = [];
}