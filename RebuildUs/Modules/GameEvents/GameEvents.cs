namespace RebuildUs.Modules.GameEvents;

public class GameEvent
{
    public GameEventType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public GameEventData Data { get; set; }
}

public static class GameEventManager
{
    private static readonly List<GameEvent> events = [];

    public static void Add(GameEventType type, GameEventData data)
    {
        events.Add(new GameEvent { Type = type, Timestamp = DateTime.Now, Data = data });
    }

    public static List<GameEvent> GetEvents()
    {
        return [.. events];
    }

    public static void ClearEvents()
    {
        events.Clear();
    }

    private static string FormatEvent(GameEvent e)
    {
        string time = e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        switch (e.Type)
        {
            case GameEventType.Kill:
                var killData = e.Data as KillEventData;
                return $"[{time}] {GetPlayerName(killData?.Killer)} killed {GetPlayerName(killData?.Victim)}";
            case GameEventType.MeetingStart:
                return $"[{time}] Meeting started";
            case GameEventType.MeetingEnd:
                var endData = e.Data as MeetingEndEventData;
                string result = endData?.Result.ToString() ?? "Unknown";
                string exiled = GetPlayerName(endData?.ExiledPlayer);
                return $"[{time}] Meeting ended - Result: {result}, Exiled: {exiled}";
            case GameEventType.RoleAction:
                var roleData = e.Data as RoleActionEventData;
                return $"[{time}] {GetPlayerName(roleData?.Player)} performed {roleData?.Action ?? "Unknown action"}";
            case GameEventType.Disconnect:
                var discData = e.Data as DisconnectEventData;
                return $"[{time}] {GetPlayerName(discData?.Player)} disconnected";
            case GameEventType.VentMove:
                var ventData = e.Data as VentMoveEventData;
                return $"[{time}] {GetPlayerName(ventData?.Player)} moved from vent {ventData?.FromVent?.name ?? "Unknown"} to {ventData?.ToVent?.name ?? "Unknown"}";
            case GameEventType.GameStart:
                var startData = e.Data as GameStartEventData;
                return $"[{time}] Game started - Map: {startData?.MapId.ToString() ?? "Unknown"}, Players: {startData?.PlayerCount ?? 0}";
            case GameEventType.GameEnd:
                var gameEndData = e.Data as GameEndEventData;
                return $"[{time}] Game ended - Winner: {gameEndData?.WinningTeam ?? "Unknown"}, Reason: {gameEndData?.Reason.ToString() ?? "Unknown"}";
            case GameEventType.AllTasksCompleted:
                var taskData = e.Data as AllTasksCompletedEventData;
                return $"[{time}] {GetPlayerName(taskData?.Player)} completed all task.";
            case GameEventType.EmergencyMeeting:
                var meetData = e.Data as EmergencyMeetingEventData;
                return $"[{time}] {GetPlayerName(meetData?.Caller)} called emergency meeting";
            case GameEventType.BodyReport:
                var reportData = e.Data as BodyReportEventData;
                return $"[{time}] {GetPlayerName(reportData?.Reporter)} reported body of {GetPlayerName(reportData?.DeadPlayer)}";
            case GameEventType.SabotageStart:
                var sabStartData = e.Data as SabotageStartEventData;
                return $"[{time}] Sabotage started: {sabStartData?.SabotageType ?? "Unknown"}";
            case GameEventType.SabotageFix:
                var sabFixData = e.Data as SabotageFixEventData;
                return $"[{time}] {GetPlayerName(sabFixData?.Player)} fixed sabotage: {sabFixData?.SabotageType ?? "Unknown"}";
            case GameEventType.PlayerStateChange:
                var stateData = e.Data as PlayerStateChangeEventData;
                return $"[{time}] {GetPlayerName(stateData?.Player)} state changed to {stateData?.FinalStatus.ToString() ?? "Unknown"}";
            case GameEventType.GameSettings:
                var settingsData = e.Data as GameSettingsEventData;
                return $"[{time}] Game settings - Map: {settingsData?.Map ?? "Unknown"}, Impostors: {settingsData?.ImpostorCount ?? 0}, Tasks: {settingsData?.TaskCount ?? 0}";
            case GameEventType.AccidentalDeath:
                var deathData = e.Data as AccidentalDeathEventData;
                return $"[{time}] {GetPlayerName(deathData?.Victim)} died accidentally due to {deathData?.Cause ?? "Unknown cause"}";
            default:
                return $"[{time}] {e.Type}";
        }
    }

    public static string GetPlayerName(PlayerControl player)
    {
        return player?.Data?.PlayerName ?? "Unknown Player";
    }

    public static string GetEventsAsString()
    {
        var sb = new StringBuilder();
        foreach (var e in events)
        {
            if (sb.Length > 0) sb.Append('\n');
            sb.Append(FormatEvent(e));
        }
        return sb.ToString();
    }
}