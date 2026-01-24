namespace RebuildUs.Modules.GameEvents.Events;

public class GameSettingsEventData : GameEventData
{
    public string Map { get; set; }
    public int ImpostorCount { get; set; }
    public int TaskCount { get; set; }
}