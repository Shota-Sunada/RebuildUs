using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text.Json;
using InnerNet;

namespace RebuildUs.Modules.Discord;

public static class DiscordModManager
{
    private static readonly HttpClient _httpClient = new();
    private static readonly string MappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "discord_mapping.json");
    private static Dictionary<string, ulong> _playerMappings = []; // FriendCode/Name -> DiscordId

    private static string[] _tokens = [];
    private static int _currentTokenIndex = 0;

    private static ulong? _statusMessageId = null;
    private static string _currentGameState = "ドロップシップ";
    private static string _exiledPlayerName = "";

    private static DiscordGatewayClient _gateway;

    public static void Initialize()
    {
        if (!RebuildUs.EnableAutoMute.Value) return;

        _tokens = [.. new[] {
            RebuildUs.DiscordBotToken.Value,
            RebuildUs.DiscordBotToken2.Value,
            RebuildUs.DiscordBotToken3.Value
        }.Where(t => !string.IsNullOrEmpty(t))];

        LoadMappings();

        if (_tokens.Length > 0)
        {
            _gateway = new DiscordGatewayClient(_tokens[0]);
            _ = _gateway.ConnectAsync();
        }
    }

    private static void LoadMappings()
    {
        if (File.Exists(MappingFile))
        {
            try
            {
                var json = File.ReadAllText(MappingFile);
                _playerMappings = JsonSerializer.Deserialize<Dictionary<string, ulong>>(json) ?? [];
            }
            catch (Exception e) { Logger.LogError($"[Discord] LoadMappings: {e.Message}"); }
        }
    }

    private static void SaveMappings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_playerMappings);
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

    private static async Task<HttpResponseMessage> SendRequest(string method, string url, object body = null, string overrideToken = null)
    {
        var token = overrideToken ?? GetNextToken();
        if (string.IsNullOrEmpty(token)) return null;

        using var request = new HttpRequestMessage(new HttpMethod(method), url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bot", token);
        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        try { return await _httpClient.SendAsync(request); }
        catch (Exception e) { Logger.LogError($"[Discord] Request error: {e.Message}"); return null; }
    }

    public static async void SetMute(ulong discordId, bool mute, bool deaf)
    {
        if (!RebuildUs.EnableAutoMute.Value) return;
        var guildId = RebuildUs.DiscordGuildId.Value;
        if (string.IsNullOrEmpty(guildId)) return;
        await SendRequest("PATCH", $"https://discord.com/api/v10/guilds/{guildId}/members/{discordId}", new { mute, deaf });
    }

    public static void UnmuteEveryone()
    {
        foreach (var mapping in _playerMappings) SetMute(mapping.Value, false, false);
    }

    public static void MuteEveryone()
    {
        foreach (var mapping in _playerMappings) SetMute(mapping.Value, true, true);
    }

    public static void OnGameStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        MuteEveryone();
        _currentGameState = "行動中";
        UpdateStatus();
    }

    public static void OnMeetingStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.Data == null) continue;
            if (_playerMappings.TryGetValue(p.Data.PlayerName, out var did))
            {
                if (!p.Data.IsDead) SetMute(did, false, false);
                else SetMute(did, true, false); // 死者スピーカーミュート解除 (Deafen off, Mute on)
            }
        }
        _currentGameState = "会議中";
        UpdateStatus();
    }

    public static void OnMeetingEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.Data == null) continue;
            if (_playerMappings.TryGetValue(p.Data.PlayerName, out var did))
            {
                if (!p.Data.IsDead) SetMute(did, true, true); // 生存者スピーカーミュート (Mute on, Deafen on)
                else SetMute(did, false, false); // 死亡者マイク・スピーカーミュート解除 (Mute off, Deafen off)
            }
        }
        _currentGameState = "行動中";
        UpdateStatus();
    }

    public static void OnExile(string name)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _currentGameState = "追放中";
        _exiledPlayerName = name;
        UpdateStatus();
    }

    public static void OnGameEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        UnmuteEveryone();
        _currentGameState = "ドロップシップ";
        UpdateStatus();
    }

    public static async void UpdateStatus()
    {
        if (!AmongUsClient.Instance.AmHost || string.IsNullOrEmpty(RebuildUs.StatusChannelId.Value)) return;

        var mapName = Helpers.GetOption(ByteOptionNames.MapId) switch
        {
            0 or 3 => TranslationController.Instance.GetString(StringNames.MapNameSkeld),
            1 => TranslationController.Instance.GetString(StringNames.MapNameMira),
            2 => TranslationController.Instance.GetString(StringNames.MapNamePolus),
            4 => TranslationController.Instance.GetString(StringNames.MapNameAirship),
            5 => TranslationController.Instance.GetString(StringNames.MapNameFungle),
            6 => SubmergedCompatibility.SUBMERGED_GUID,
            _ => "Unknown"
        };
        var roomId = GameCode.IntToGameName(AmongUsClient.Instance.GameId) ?? "---";
        var count = PlayerControl.AllPlayerControls.Count;

        string desc = _currentGameState switch
        {
            "ドロップシップ" => "ゲーム開始を待機中",
            "行動中" => "各自タスクやキルを行う",
            "追放中" => string.IsNullOrEmpty(_exiledPlayerName) ? "誰かが追放された" : $"{_exiledPlayerName}が追放された",
            "会議中" => "話し合いをしている",
            _ => ""
        };

        var embed = new
        {
            title = "Among Us ルーム情報",
            color = 3447003,
            fields = new[] {
                new { name = "マップ名", value = mapName, inline = true },
                new { name = "ルームID", value = roomId, inline = true },
                new { name = "状態", value = _currentGameState, inline = true },
                new { name = "状態の説明", value = desc, inline = false },
                new { name = "部屋人数", value = $"{count}/{Helpers.GetOption(Int32OptionNames.MaxPlayers)}", inline = true }
            },
            description = GetPlayerListString()
        };

        var components = new[] {
            new { type = 1, components = new[] {
                new { type = 2, style = 1, label = "アカウント連携", custom_id = "start_link" }
            } }
        };

        var body = new { embeds = new[] { embed }, components };
        var url = $"https://discord.com/api/v10/channels/{RebuildUs.StatusChannelId.Value}/messages" + (_statusMessageId == null ? "" : $"/{_statusMessageId}");
        var resp = await SendRequest(_statusMessageId == null ? "POST" : "PATCH", url, body, RebuildUs.DiscordBotToken.Value);
        if (_statusMessageId == null && resp != null && resp.IsSuccessStatusCode)
        {
            var json = await resp.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("id", out var id)) _statusMessageId = ulong.Parse(id.GetString()!);
        }
    }

    private static string GetPlayerListString()
    {
        var linked = new List<string>();
        var unlinked = new List<string>();
        foreach (var p in PlayerControl.AllPlayerControls)
        {
            if (p.Data == null) continue;
            var name = p.Data.PlayerName;
            if (_playerMappings.ContainsKey(name)) linked.Add($":white_check_mark: {name}");
            else unlinked.Add($":x: {name}");
        }
        var sb = new StringBuilder("**プレイヤー名一覧:**\n");
        foreach (var s in linked) sb.AppendLine(s);
        foreach (var s in unlinked) sb.AppendLine(s);
        return sb.ToString();
    }

    public static void AddMapping(string name, ulong did)
    {
        _playerMappings[name] = did;
        SaveMappings();
        UpdateStatus();
    }

    private class DiscordGatewayClient
    {
        private ClientWebSocket _ws;
        private readonly string _token;
        private int? _s;
        public DiscordGatewayClient(string t) => _token = t;
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
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    var res = await _ws.ReceiveAsync(new ArraySegment<byte>(buf), CancellationToken.None);
                    if (res.MessageType == WebSocketMessageType.Text)
                    {
                        var json = Encoding.UTF8.GetString(buf, 0, res.Count);
                        Handle(json);
                    }
                }
                catch { break; }
            }
        }
        private void Handle(string json)
        {
            try
            {
                var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("s", out var s) && s.ValueKind != JsonValueKind.Null) _s = s.GetInt32();
                var op = doc.RootElement.GetProperty("op").GetInt32();
                if (op == 10)
                { // Hello
                    var hb = doc.RootElement.GetProperty("d").GetProperty("heartbeat_interval").GetInt32();
                    _ = Heartbeat(hb);
                    Send(new { op = 2, d = new { token = _token, intents = 1, properties = new { os = "win", browser = "rb", device = "rb" } } });
                }
                else if (op == 0)
                { // Dispatch
                    var t = doc.RootElement.GetProperty("t").GetString();
                    if (t == "INTERACTION_CREATE") HandleInteraction(doc.RootElement.GetProperty("d"));
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
        private void Send(object o)
        {
            try
            {
                var j = JsonSerializer.Serialize(o);
                _ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(j)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch { }
        }
        private void HandleInteraction(JsonElement d)
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
                    foreach (var p in PlayerControl.AllPlayerControls)
                    {
                        if (p == null || p.Data == null) continue;
                        optionsList.Add(new { label = p.Data.PlayerName, value = p.Data.PlayerName });
                    }
                    var options = optionsList.ToArray();

                    if (options.Length == 0) return;
                    Respond(id, token, new
                    {
                        type = 4,
                        data = new
                        {
                            content = "連携するプレイヤーを選択してください:",
                            flags = 64,
                            components = new[] { new { type = 1, components = new[] { new { type = 3, custom_id = "select_player", options = options } } } }
                        }
                    });
                }
                else if (customId == "select_player")
                {
                    var name = d.GetProperty("data").GetProperty("values")[0].GetString();
                    var did = ulong.Parse(d.GetProperty("member").GetProperty("user").GetProperty("id").GetString());
                    AddMapping(name, did);
                    Respond(id, token, new { type = 4, data = new { content = $"{name} と連携しました！", flags = 64 } });
                }
            }
        }
        private async void Respond(string id, string token, object body)
        {
            await SendRequest("POST", $"https://discord.com/api/v10/interactions/{id}/{token}/callback", body);
        }
    }
}
