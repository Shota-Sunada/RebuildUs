namespace RebuildUs.Modules.GameEvents;

internal sealed class GameEvent
{
    internal GameEventType Type { get; set; }
    internal DateTime Timestamp { get; set; }
    internal GameEventData Data { get; set; }
}

internal static class GameEventManager
{
    private static readonly List<GameEvent> Events = [];

    internal static void Add(GameEventType type, GameEventData data)
    {
        Events.Add(new() { Type = type, Timestamp = DateTime.Now, Data = data });
    }

    internal static List<GameEvent> GetEvents()
    {
        return [.. Events];
    }

    internal static void ClearEvents()
    {
        Events.Clear();
    }

    private static string FormatEvent(GameEvent e)
    {
        string time = e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
        switch (e.Type)
        {
            case GameEventType.Kill:
                KillEventData killData = e.Data as KillEventData;
                return $"[{time}] {GetPlayerName(killData?.Killer)} killed {GetPlayerName(killData?.Victim)}";
            case GameEventType.MeetingStart:
                return $"[{time}] Meeting started";
            case GameEventType.MeetingEnd:
                MeetingEndEventData endData = e.Data as MeetingEndEventData;
                string result = endData?.Result.ToString() ?? "Unknown";
                string exiled = GetPlayerName(endData?.ExiledPlayer);
                return $"[{time}] Meeting ended - Result: {result}, Exiled: {exiled}";
            case GameEventType.RoleAction:
                RoleActionEventData roleData = e.Data as RoleActionEventData;
                return $"[{time}] {GetPlayerName(roleData?.Player)} performed {roleData?.Action ?? "Unknown action"}";
            case GameEventType.Disconnect:
                DisconnectEventData discData = e.Data as DisconnectEventData;
                return $"[{time}] {GetPlayerName(discData?.Player)} disconnected";
            case GameEventType.VentMove:
                VentMoveEventData ventData = e.Data as VentMoveEventData;
                return $"[{time}] {GetPlayerName(ventData?.Player)} moved from vent {ventData?.FromVent?.name ?? "Unknown"} to {ventData?.ToVent?.name ?? "Unknown"}";
            case GameEventType.GameStart:
                GameStartEventData startData = e.Data as GameStartEventData;
                return $"[{time}] Game started - Map: {startData?.MapId.ToString() ?? "Unknown"}, Players: {startData?.PlayerCount ?? 0}";
            case GameEventType.GameEnd:
                GameEndEventData gameEndData = e.Data as GameEndEventData;
                return $"[{time}] Game ended - Winner: {gameEndData?.WinningTeam ?? "Unknown"}, Reason: {gameEndData?.Reason.ToString() ?? "Unknown"}";
            case GameEventType.AllTasksCompleted:
                AllTasksCompletedEventData taskData = e.Data as AllTasksCompletedEventData;
                return $"[{time}] {GetPlayerName(taskData?.Player)} completed all task.";
            case GameEventType.EmergencyMeeting:
                EmergencyMeetingEventData meetData = e.Data as EmergencyMeetingEventData;
                return $"[{time}] {GetPlayerName(meetData?.Caller)} called emergency meeting";
            case GameEventType.BodyReport:
                BodyReportEventData reportData = e.Data as BodyReportEventData;
                return $"[{time}] {GetPlayerName(reportData?.Reporter)} reported body of {GetPlayerName(reportData?.DeadPlayer)}";
            case GameEventType.SabotageStart:
                SabotageStartEventData sabStartData = e.Data as SabotageStartEventData;
                return $"[{time}] Sabotage started: {sabStartData?.SabotageType ?? "Unknown"}";
            case GameEventType.SabotageFix:
                SabotageFixEventData sabFixData = e.Data as SabotageFixEventData;
                return $"[{time}] {GetPlayerName(sabFixData?.Player)} fixed sabotage: {sabFixData?.SabotageType ?? "Unknown"}";
            case GameEventType.PlayerStateChange:
                PlayerStateChangeEventData stateData = e.Data as PlayerStateChangeEventData;
                return $"[{time}] {GetPlayerName(stateData?.Player)} state changed to {stateData?.FinalStatus.ToString() ?? "Unknown"}";
            case GameEventType.GameSettings:
                GameSettingsEventData settingsData = e.Data as GameSettingsEventData;
                return $"[{time}] Game settings - Map: {settingsData?.Map ?? "Unknown"}, Impostors: {settingsData?.ImpostorCount ?? 0}, Tasks: {settingsData?.TaskCount ?? 0}";
            case GameEventType.AccidentalDeath:
                AccidentalDeathEventData deathData = e.Data as AccidentalDeathEventData;
                return $"[{time}] {GetPlayerName(deathData?.Victim)} died accidentally due to {deathData?.Cause ?? "Unknown cause"}";
            default:
                return $"[{time}] {e.Type}";
        }
    }

    internal static string GetPlayerName(PlayerControl player)
    {
        return player?.Data?.PlayerName ?? "Unknown Player";
    }

    internal static string GetEventsAsString()
    {
        StringBuilder sb = new();
        foreach (GameEvent e in Events)
        {
            if (sb.Length > 0) sb.Append('\n');
            sb.Append(FormatEvent(e));
        }

        return sb.ToString();
    }
}