namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class GameEndEventData : GameEventData
{
    internal string WinningTeam { get; set; }
    internal GameOverReason Reason { get; set; }
}