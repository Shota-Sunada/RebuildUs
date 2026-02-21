namespace RebuildUs.Modules.GameEvents.Events;

internal abstract class GameSettingsEventData : GameEventData
{
    internal string Map { get; set; }
    internal int ImpostorCount { get; set; }
    internal int TaskCount { get; set; }
}