using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text.Json;
using InnerNet;

namespace RebuildUs.Modules.Discord;

public static class DiscordModManager
{
    private static readonly HttpClient _httpClient = new();
    private static readonly string MappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "discord_mapping.json");
    private static Dictionary<string, ulong> _playerMappings = []; // FriendCode -> DiscordId

    private static string[] _tokens = [];
    private static int _currentTokenIndex = 0;

    private static ulong? _statusMessageId = null;
    private static string _currentGameState = "ドロップシップ";
    private static string _exiledPlayerName = "";

    private static DiscordGatewayClient _gateway;

    public static void Initialize()
    {
        if (_gateway != null) return;
        if (!ModMapOptions.EnableDiscordAutoMute && !ModMapOptions.EnableSendFinalStatusToDiscord && !ModMapOptions.EnableDiscordEmbed) return;

        _tokens = [.. new[] {
            RebuildUs.DiscordBotToken.Value,
            RebuildUs.DiscordBotToken2.Value,
            RebuildUs.DiscordBotToken3.Value
        }.Where(t => !string.IsNullOrEmpty(t))];

        if (_tokens.Length == 0) return;

        LoadMappings();

        _gateway = new DiscordGatewayClient(_tokens[0]);
        _ = _gateway.ConnectAsync();
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

        try { return await _httpClient.SendAsync(request).ConfigureAwait(false); }
        catch (Exception e) { Logger.LogError($"[Discord] Request error: {e.Message}"); return null; }
    }

    public static async void SetMute(ulong discordId, bool mute, bool deaf)
    {
        if (!ModMapOptions.EnableDiscordAutoMute) return;
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
        _currentGameState = "行動中";
        MuteEveryone();
        UpdateStatus();
    }

    public static void OnMeetingStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _currentGameState = "会議中";
        foreach (var p in PlayerControl.AllPlayerControls) UpdatePlayerMute(p);
        UpdateStatus();
    }

    public static void OnMeetingEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _currentGameState = "行動中";
        foreach (var p in PlayerControl.AllPlayerControls) UpdatePlayerMute(p);
        UpdateStatus();
    }

    public static void OnExile(string name)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _currentGameState = "追放中";
        _exiledPlayerName = name;
        foreach (var p in PlayerControl.AllPlayerControls) UpdatePlayerMute(p);
        UpdateStatus();
    }

    public static void OnGameEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _currentGameState = "ドロップシップ";
        UnmuteEveryone();
        UpdateStatus();
    }

    public static void UpdatePlayerMute(PlayerControl p)
    {
        if (!AmongUsClient.Instance.AmHost || p == null || p.Data == null || string.IsNullOrEmpty(p.FriendCode)) return;
        if (!_playerMappings.TryGetValue(p.FriendCode, out var did)) return;

        bool mute = false;
        bool deaf = false;

        switch (_currentGameState)
        {
            case "行動中":
            case "追放中":
                if (!p.Data.IsDead) { mute = true; deaf = true; }
                else { mute = false; deaf = false; }
                break;
            case "会議中":
                if (!p.Data.IsDead) { mute = false; deaf = false; }
                else { mute = true; deaf = false; }
                break;
            case "ドロップシップ":
                mute = false;
                deaf = false;
                break;
        }

        SetMute(did, mute, deaf);
    }

    public static async void UpdateStatus()
    {
        if (!ModMapOptions.EnableDiscordEmbed || !AmongUsClient.Instance.AmHost || string.IsNullOrEmpty(RebuildUs.StatusChannelId.Value)) return;
        if (_currentGameState == "ドロップシップ" && !ModMapOptions.EnableSendFinalStatusToDiscord) return;

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
            var json = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
            using var doc = JsonDocument.Parse(json);
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
            if (!string.IsNullOrEmpty(p.FriendCode) && _playerMappings.ContainsKey(p.FriendCode)) linked.Add($":white_check_mark: {name}");
            else unlinked.Add($":x: {name}");
        }
        var sb = new StringBuilder("**プレイヤー名一覧:**\n");
        foreach (var s in linked) sb.AppendLine(s);
        foreach (var s in unlinked) sb.AppendLine(s);
        return sb.ToString();
    }

    public static bool TryGetDiscordId(string friendCode, out ulong did)
    {
        return _playerMappings.TryGetValue(friendCode, out did);
    }

    public static void AddMapping(string friendCode, ulong did)
    {
        _playerMappings[friendCode] = did;
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
                        foreach (var p in PlayerControl.AllPlayerControls)
                        {
                            if (p == null || p.Data == null || string.IsNullOrEmpty(p.FriendCode)) continue;
                            optionsList.Add(new { label = p.Data.PlayerName, value = p.FriendCode });
                        }
                        var options = optionsList.ToArray();

                        if (options.Length == 0)
                        {
                            Respond(id, token, new { type = 4, data = new { content = "プレイヤーが見つかりません。", flags = 64 } });
                            return;
                        }
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
                        var friendCode = d.GetProperty("data").GetProperty("values")[0].GetString();
                        var did = ulong.Parse(d.GetProperty("member").GetProperty("user").GetProperty("id").GetString());
                        AddMapping(friendCode, did);
                        Respond(id, token, new { type = 4, data = new { content = $"{friendCode} と連携しました！", flags = 64 } });
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