using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text.Json;

namespace RebuildUs.Modules.Discord;

public static class DiscordModManager
{
    private static readonly HttpClient _httpClient = new();
    private static readonly string MappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "discord_mapping.json");
    internal static Dictionary<string, ulong> PlayerMappings = []; // FriendCode -> DiscordId
    internal static Dictionary<ulong, string> PlayerVoiceStates = []; // DiscordId -> ChannelId

    private static string[] _tokens = [];
    private static int _currentTokenIndex = 0;

    internal static string CurrentGameState = "ドロップシップ";
    internal static string ExiledPlayerName = "";
    internal static byte ExiledPlayerId = 255;

    private static readonly List<DiscordGatewayClient> _gateways = [];

    public static void Initialize()
    {
        if (_gateways.Count > 0) return;
        if (!MapSettings.EnableDiscordAutoMute && !MapSettings.EnableDiscordEmbed) return;

        _tokens = [.. new[] {
            RebuildUs.DiscordBotToken.Value,
            RebuildUs.DiscordBotToken2.Value,
            RebuildUs.DiscordBotToken3.Value
        }.Where(t => !string.IsNullOrEmpty(t))];

        if (_tokens.Length == 0) return;

        LoadMappings();

        // Connect all bots
        for (int i = 0; i < _tokens.Length; i++)
        {
            var token = _tokens[i];
            // Only the first bot handles events (Interactions/VoiceStateUpdates)
            // to avoid duplicate processing if they are in the same server/channel.
            // But if they are different bots, they only receive interactions for themselves.
            // VoiceStateUpdates are sent to all bots in the guild.
            // So we enable event handling only for the primary bot to keep logic simple.
            var gateway = new DiscordGatewayClient(token, i == 0);
            _ = gateway.ConnectAsync();
            _gateways.Add(gateway);
        }
    }

    private static void LoadMappings()
    {
        if (File.Exists(MappingFile))
        {
            try
            {
                var json = File.ReadAllText(MappingFile);
                PlayerMappings = JsonSerializer.Deserialize<Dictionary<string, ulong>>(json) ?? [];
            }
            catch (Exception e) { Logger.LogError($"[Discord] LoadMappings: {e.Message}"); }
        }
    }

    private static void SaveMappings()
    {
        try
        {
            var json = JsonSerializer.Serialize(PlayerMappings);
            File.WriteAllText(MappingFile, json);
        }
        catch (Exception e) { Logger.LogError($"[Discord] SaveMappings: {e.Message}"); }
    }

    private static string GetNextToken()
    {
        if (_tokens.Length == 0) return "";
        var tok = _tokens[_currentTokenIndex];
        _currentTokenIndex = (_currentTokenIndex + 1) % _tokens.Length;
        return tok;
    }

    internal static async Task<HttpResponseMessage> SendRequest(string method, string url, object body = null, string overrideToken = null)
    {
        var token = overrideToken ?? GetNextToken();
        if (string.IsNullOrEmpty(token)) return null;

        using var request = new HttpRequestMessage(new HttpMethod(method), url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", token);
        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        try { return await _httpClient.SendAsync(request).ConfigureAwait(false); }
        catch (Exception e) { Logger.LogError($"[Discord] Request error: {e.Message}"); return null; }
    }

    public static void OnGameStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "行動中";
        ExiledPlayerName = "";
        ExiledPlayerId = 255;
        DiscordAutoMuteManager.MuteEveryone();
        DiscordEmbedManager.UpdateStatus();
    }

    public static void OnMeetingStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "会議中";
        ExiledPlayerName = "";
        ExiledPlayerId = 255;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    public static void OnMeetingEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "行動中";
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    public static void OnExile(string name, byte playerId)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "追放中";
        ExiledPlayerName = name;
        ExiledPlayerId = playerId;
        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    public static void OnGameEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "ドロップシップ";
        DiscordAutoMuteManager.UnmuteEveryone();
        DiscordEmbedManager.UpdateStatus();
    }

    public static void OnQuitGame()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        DiscordAutoMuteManager.UnmuteEveryone();
    }

    public static void OnPlayerLeft(string friendCode)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        DiscordAutoMuteManager.OnPlayerLeft(friendCode);
        DiscordEmbedManager.UpdateStatus();
    }

    public static bool TryGetDiscordId(string friendCode, out ulong did)
    {
        return PlayerMappings.TryGetValue(friendCode, out did);
    }

    public static string GetIdentifier(PlayerControl p)
    {
        if (p == null || p.Data == null) return null;
        return !string.IsNullOrEmpty(p.FriendCode) ? p.FriendCode : p.Data.PlayerName;
    }

    public static void AddMapping(string friendCode, ulong did)
    {
        PlayerMappings[friendCode] = did;
        SaveMappings();
        DiscordEmbedManager.UpdateStatus();
    }

    private class DiscordGatewayClient
    {
        private ClientWebSocket _ws;
        private readonly string _token;
        private readonly bool _handleEvents;
        private int? _s;

        public DiscordGatewayClient(string t, bool handleEvents)
        {
            _token = t;
            _handleEvents = handleEvents;
        }

        public async Task ConnectAsync()
        {
            try
            {
                _ws = new ClientWebSocket();
                await _ws.ConnectAsync(new Uri("wss://gateway.discord.gg/?v=10&encoding=json"), CancellationToken.None);
                _ = ReceiveLoop();
            }
            catch (Exception e) { Logger.LogError($"[Discord] WS Connect: {e.Message}"); }
        }
        private async Task ReceiveLoop()
        {
            var buf = new byte[1024 * 16];
            using var ms = new MemoryStream();
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    ms.SetLength(0);
                    WebSocketReceiveResult res;
                    do
                    {
                        res = await _ws.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
                        if (res.MessageType == WebSocketMessageType.Close) break;
                        ms.Write(buf, 0, res.Count);
                    } while (!res.EndOfMessage);

                    if (res.MessageType == WebSocketMessageType.Close) break;

                    var json = Encoding.UTF8.GetString(ms.ToArray());
                    Handle(json);
                }
                catch (Exception e)
                {
                    Logger.LogError($"[Discord] ReceiveLoop error: {e.Message}");
                    break;
                }
            }
        }
        private void Handle(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("s", out var s) && s.ValueKind != JsonValueKind.Null) _s = s.GetInt32();
                var op = doc.RootElement.GetProperty("op").GetInt32();
                if (op == 10)
                { // Hello
                    var hb = doc.RootElement.GetProperty("d").GetProperty("heartbeat_interval").GetInt32();
                    _ = Heartbeat(hb);
                    Send(new { Op = 2, D = new { Token = _token, Intents = 1 | 128, Properties = new { Os = "win", Browser = "rb", Device = "rb" } } });
                }
                else if (op == 0)
                { // Dispatch
                    if (!_handleEvents) return;

                    var t = doc.RootElement.GetProperty("t").GetString();
                    if (t == "INTERACTION_CREATE") HandleInteraction(doc.RootElement.GetProperty("d"));
                    else if (t == "VOICE_STATE_UPDATE")
                    {
                        var d = doc.RootElement.GetProperty("d");
                        var uid = ulong.Parse(d.GetProperty("user_id").GetString()!);
                        var cid = d.TryGetProperty("channel_id", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetString() : null;
                        if (cid != null) PlayerVoiceStates[uid] = cid;
                        else PlayerVoiceStates.Remove(uid);
                    }
                    else if (t == "GUILD_CREATE")
                    {
                        var d = doc.RootElement.GetProperty("d");
                        if (d.TryGetProperty("voice_states", out var vs))
                        {
                            foreach (var s2 in vs.EnumerateArray())
                            {
                                var uid = ulong.Parse(s2.GetProperty("user_id").GetString()!);
                                var cid = s2.TryGetProperty("channel_id", out var c) && c.ValueKind != JsonValueKind.Null ? c.GetString() : null;
                                if (cid != null) PlayerVoiceStates[uid] = cid;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private async Task Heartbeat(int ms)
        {
            while (_ws.State == WebSocketState.Open)
            {
                await Task.Delay(ms);
                Send(new { op = 1, d = _s });
            }
        }
        private async void Send(object o)
        {
            try
            {
                var j = JsonSerializer.Serialize(o);
                var bytes = Encoding.UTF8.GetBytes(j);
                if (_ws.State == WebSocketState.Open)
                {
                    await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[Discord] Send error: {e.Message}");
            }
        }
        private async void HandleInteraction(JsonElement d)
        {
            try
            {
                var id = d.GetProperty("id").GetString();
                var token = d.GetProperty("token").GetString();
                var type = d.GetProperty("type").GetInt32();
                if (type == 3)
                { // Component
                    var customId = d.GetProperty("data").GetProperty("custom_id").GetString();
                    if (customId == "start_link")
                    {
                        var optionsList = new List<object>();
                        foreach (var p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        {
                            var identifier = GetIdentifier(p);
                            if (string.IsNullOrEmpty(identifier)) continue;
                            optionsList.Add(new { label = p.Data.PlayerName, value = identifier });
                        }
                        var options = optionsList.ToArray();

                        if (options.Length == 0)
                        {
                            Respond(id, token, new { type = 4, Data = new { Content = "プレイヤーが見つかりません。", Flags = 64 } });
                            return;
                        }
                        Respond(id, token, new
                        {
                            type = 4,
                            data = new
                            {
                                content = "連携するプレイヤーを選択してください:",
                                flags = 64,
                                components = new[] { new { type = 1, components = new[] { new { type = 3, custom_id = "select_player", Options = options } } } }
                            }
                        });
                    }
                    else if (customId == "select_player")
                    {
                        var friendCode = d.GetProperty("data").GetProperty("values")[0].GetString();
                        var did = ulong.Parse(d.GetProperty("member").GetProperty("user").GetProperty("id").GetString());
                        AddMapping(friendCode, did);
                        Respond(id, token, new { type = 4, Data = new { Content = $"{friendCode} と連携しました！", flags = 64 } });
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"[Discord] HandleInteraction error: {e.Message}");
            }
        }
        private async void Respond(string id, string token, object body)
        {
            await SendRequest("POST", $"https://discord.com/api/v10/interactions/{id}/{token}/callback", body, _token);
        }
    }
}