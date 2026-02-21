using System.Net.WebSockets;
using System.Text.Json;

namespace RebuildUs.Modules.Discord;

internal static class DiscordModManager
{
    private static readonly HttpClient HttpClient = new();
    private static readonly string MappingFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "discord_mapping.json");
    internal static Dictionary<string, ulong> PlayerMappings = []; // FriendCode -> DiscordId
    internal static readonly Dictionary<ulong, string> PlayerVoiceStates = []; // DiscordId -> ChannelId

    private static string[] _tokens = [];
    private static int _currentTokenIndex;

    internal static string CurrentGameState = "ドロップシップ";
    internal static string ExiledPlayerName = "";
    internal static byte ExiledPlayerId = 255;

    private static readonly List<DiscordGatewayClient> Gateways = [];

    internal static void Initialize()
    {
        if (Gateways.Count > 0) return;
        if (!MapSettings.EnableDiscordAutoMute && !MapSettings.EnableDiscordEmbed) return;

        _tokens =
        [
            .. new[]
            {
                RebuildUs.DiscordBotToken.Value,
                RebuildUs.DiscordBotToken2.Value,
                RebuildUs.DiscordBotToken3.Value,
            }.Where(t => !string.IsNullOrEmpty(t)),
        ];

        if (_tokens.Length == 0) return;

        LoadMappings();

        // Connect all bots
        for (int i = 0; i < _tokens.Length; i++)
        {
            string token = _tokens[i];
            // Only the first bot handles events (Interactions/VoiceStateUpdates)
            // to avoid duplicate processing if they are in the same server/channel.
            // But if they are different bots, they only receive interactions for themselves.
            // VoiceStateUpdates are sent to all bots in the guild.
            // So we enable event handling only for the primary bot to keep logic simple.
            DiscordGatewayClient gateway = new(token, i == 0);
            _ = gateway.ConnectAsync();
            Gateways.Add(gateway);
        }
    }

    private static void LoadMappings()
    {
        if (!File.Exists(MappingFile)) return;
        try
        {
            string json = File.ReadAllText(MappingFile);
            PlayerMappings = JsonSerializer.Deserialize<Dictionary<string, ulong>>(json) ?? [];
        }
        catch (Exception e) { Logger.LogError($"[Discord] LoadMappings: {e.Message}"); }
    }

    private static void SaveMappings()
    {
        try
        {
            string json = JsonSerializer.Serialize(PlayerMappings);
            File.WriteAllText(MappingFile, json);
        }
        catch (Exception e) { Logger.LogError($"[Discord] SaveMappings: {e.Message}"); }
    }

    private static string GetNextToken()
    {
        if (_tokens.Length == 0) return "";
        string tok = _tokens[_currentTokenIndex];
        _currentTokenIndex = (_currentTokenIndex + 1) % _tokens.Length;
        return tok;
    }

    internal static async Task<HttpResponseMessage> SendRequest(string method, string url, object body = null, string overrideToken = null)
    {
        string token = overrideToken ?? GetNextToken();
        if (string.IsNullOrEmpty(token)) return null;

        using HttpRequestMessage request = new(new(method), url);
        request.Headers.Authorization = new("Bot", token);
        if (body != null)
            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

        try { return await HttpClient.SendAsync(request).ConfigureAwait(false); }
        catch (Exception e)
        {
            Logger.LogError($"[Discord] Request error: {e.Message}");
            return null;
        }
    }

    internal static void OnGameStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "行動中";
        ExiledPlayerName = "";
        ExiledPlayerId = 255;
        DiscordAutoMuteManager.MuteEveryone();
        DiscordEmbedManager.UpdateStatus();
    }

    internal static void OnMeetingStart()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "会議中";
        ExiledPlayerName = "";
        ExiledPlayerId = 255;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    internal static void OnMeetingEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "行動中";
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    internal static void OnExile(string name, byte playerId)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "追放中";
        ExiledPlayerName = name;
        ExiledPlayerId = playerId;
        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator()) DiscordAutoMuteManager.UpdatePlayerMute(p);
        DiscordEmbedManager.UpdateStatus();
    }

    internal static void OnGameEnd()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        CurrentGameState = "ドロップシップ";
        DiscordAutoMuteManager.UnmuteEveryone();
        DiscordEmbedManager.UpdateStatus();
    }

    internal static void OnQuitGame()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        DiscordAutoMuteManager.UnmuteEveryone();
    }

    internal static void OnPlayerLeft(string friendCode)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        DiscordAutoMuteManager.OnPlayerLeft(friendCode);
        DiscordEmbedManager.UpdateStatus();
    }

    internal static bool TryGetDiscordId(string friendCode, out ulong did)
    {
        return PlayerMappings.TryGetValue(friendCode, out did);
    }

    internal static string GetIdentifier(PlayerControl p)
    {
        if (p == null || p.Data == null) return null;
        return !string.IsNullOrEmpty(p.FriendCode) ? p.FriendCode : p.Data.PlayerName;
    }

    private static void AddMapping(string friendCode, ulong did)
    {
        PlayerMappings[friendCode] = did;
        SaveMappings();
        DiscordEmbedManager.UpdateStatus();
    }

    private sealed class DiscordGatewayClient
    {
        private readonly bool _handleEvents;
        private readonly string _token;
        private int? _s;
        private ClientWebSocket _ws;

        internal DiscordGatewayClient(string t, bool handleEvents)
        {
            _token = t;
            _handleEvents = handleEvents;
        }

        internal async Task ConnectAsync()
        {
            try
            {
                _ws = new();
                await _ws.ConnectAsync(new("wss://gateway.discord.gg/?v=10&encoding=json"), CancellationToken.None);
                _ = ReceiveLoop();
            }
            catch (Exception e) { Logger.LogError($"[Discord] WS Connect: {e.Message}"); }
        }

        private async Task ReceiveLoop()
        {
            byte[] buf = new byte[1024 * 16];
            using MemoryStream ms = new();
            while (_ws.State == WebSocketState.Open)
            {
                try
                {
                    ms.SetLength(0);
                    WebSocketReceiveResult res;
                    do
                    {
                        res = await _ws.ReceiveAsync(new(buf), CancellationToken.None);
                        if (res.MessageType == WebSocketMessageType.Close) break;
                        ms.Write(buf, 0, res.Count);
                    } while (!res.EndOfMessage);

                    if (res.MessageType == WebSocketMessageType.Close) break;

                    string json = Encoding.UTF8.GetString(ms.ToArray());
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
                using JsonDocument doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("s", out JsonElement s) && s.ValueKind != JsonValueKind.Null) _s = s.GetInt32();
                int op = doc.RootElement.GetProperty("op").GetInt32();
                switch (op)
                {
                    case 10:
                    {
                        // Hello
                        int hb = doc.RootElement.GetProperty("d").GetProperty("heartbeat_interval").GetInt32();
                        _ = Heartbeat(hb);
                        Send(new { Op = 2, D = new { Token = _token, Intents = 1 | 128, Properties = new { Os = "win", Browser = "rb", Device = "rb" } } });
                        break;
                    }
                    // Dispatch
                    case 0 when !_handleEvents:
                        return;
                    case 0:
                    {
                        string t = doc.RootElement.GetProperty("t").GetString();
                        switch (t)
                        {
                            case "INTERACTION_CREATE":
                                HandleInteraction(doc.RootElement.GetProperty("d"));
                                break;
                            case "VOICE_STATE_UPDATE":
                            {
                                JsonElement d = doc.RootElement.GetProperty("d");
                                ulong uid = ulong.Parse(d.GetProperty("user_id").GetString()!);
                                string cid = d.TryGetProperty("channel_id", out JsonElement c) && c.ValueKind != JsonValueKind.Null ? c.GetString() : null;
                                if (cid != null) PlayerVoiceStates[uid] = cid;
                                else PlayerVoiceStates.Remove(uid);
                                break;
                            }
                            case "GUILD_CREATE":
                            {
                                JsonElement d = doc.RootElement.GetProperty("d");
                                if (d.TryGetProperty("voice_states", out JsonElement vs))
                                {
                                    foreach (JsonElement s2 in vs.EnumerateArray())
                                    {
                                        ulong uid = ulong.Parse(s2.GetProperty("user_id").GetString()!);
                                        string cid = s2.TryGetProperty("channel_id", out JsonElement c) && c.ValueKind != JsonValueKind.Null ? c.GetString() : null;
                                        if (cid != null) PlayerVoiceStates[uid] = cid;
                                    }
                                }

                                break;
                            }
                        }

                        break;
                    }
                }
            }
            catch
            {
                // ignored
            }
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
                string j = JsonSerializer.Serialize(o);
                byte[] bytes = Encoding.UTF8.GetBytes(j);
                if (_ws.State == WebSocketState.Open) await _ws.SendAsync(new(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
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
                string id = d.GetProperty("id").GetString();
                string token = d.GetProperty("token").GetString();
                int type = d.GetProperty("type").GetInt32();
                if (type != 3) return;
                // Component
                string customId = d.GetProperty("data").GetProperty("custom_id").GetString();
                switch (customId)
                {
                    case "start_link":
                    {
                        List<object> optionsList = new();
                        foreach (PlayerControl p in PlayerControl.AllPlayerControls.GetFastEnumerator())
                        {
                            string identifier = GetIdentifier(p);
                            if (string.IsNullOrEmpty(identifier)) continue;
                            optionsList.Add(new { label = p.Data.PlayerName, value = identifier });
                        }

                        object[] options = optionsList.ToArray();

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
                                components = new[] { new { type = 1, components = new[] { new { type = 3, custom_id = "select_player", Options = options } } } },
                            },
                        });
                        break;
                    }
                    case "select_player":
                    {
                        string friendCode = d.GetProperty("data").GetProperty("values")[0].GetString();
                        ulong did = ulong.Parse(d.GetProperty("member").GetProperty("user").GetProperty("id").GetString());
                        AddMapping(friendCode, did);
                        Respond(id, token, new { type = 4, Data = new { Content = $"{friendCode} と連携しました！", flags = 64 } });
                        break;
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