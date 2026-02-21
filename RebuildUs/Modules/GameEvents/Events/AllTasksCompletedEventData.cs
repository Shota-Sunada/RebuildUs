namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class AllTasksCompletedEventData : GameEventData
{
    internal PlayerControl Player { get; set; }
}