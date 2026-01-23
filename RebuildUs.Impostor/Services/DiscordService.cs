using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace RebuildUs.Impostor.Services;

public interface IDiscordService
{
    Task StartAsync();
    Task StopAsync();
    Task SetMuteAsync(ulong discordId, bool mute, bool deaf);
    Task SetMuteAllAsync(IEnumerable<ulong> discordIds, bool mute, bool deaf);
    Task SendMessageToDiscordAsync(string message);
    Task UpdateStatusAsync(string roomCode, int playerCount, int maxPlayers, string mapName, string gameState, string playerLinkStatus);
    Task SetActiveRoomCodeAsync(string roomCode);
    Task ClearActiveRoomCodeAsync();
    bool IsDisabled { get; set; }
    bool IsMessageDisabled { get; set; }
    string? TargetRoomCode { get; set; }
    ulong? OverrideTextChannelId { get; set; }
}

public class DiscordService : IDiscordService
{
    private readonly List<DiscordSocketClient> _clients = [];
    private readonly Models.DiscordConfig _config;
    private readonly ILogger<DiscordService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private string? _activeRoomCode;
    private ulong? _statusMessageId;
    private string? _targetRoomCode;
    private bool _isAutoTrack = true;

    public bool IsDisabled { get; set; }
    public bool IsMessageDisabled { get; set; }
    public ulong? OverrideTextChannelId { get; set; }

    public string? TargetRoomCode
    {
        get => _targetRoomCode;
        set
        {
            _targetRoomCode = value?.ToUpper();
            _isAutoTrack = value == null;
        }
    }

    public DiscordService(ILogger<DiscordService> logger, IOptions<Models.DiscordConfig> config, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _config = config.Value;
        _serviceProvider = serviceProvider;
        IsDisabled = _config.DisableDiscord;
        IsMessageDisabled = false;

        if (_config.DisableDiscord)
        {
            _logger.LogInformation("Discord service is disabled via configuration.");
            return;
        }

        // Add main client
        _clients.Add(CreateClient(true));

        // Add worker clients
        foreach (var _ in _config.WorkerTokens)
        {
            _clients.Add(CreateClient(false));
        }
    }

    private DiscordSocketClient CreateClient(bool isMain)
    {
        var intents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMembers;
        if (isMain)
        {
            intents |= GatewayIntents.GuildMessages | GatewayIntents.MessageContent;
        }

        var client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = intents
        });
        client.Log += LogAsync;
        client.Ready += () => ReadyAsync(client);
        if (isMain)
        {
            client.MessageReceived += MessageReceivedAsync;
            client.ButtonExecuted += ButtonExecutedAsync;
        }
        return client;
    }

    private async Task ButtonExecutedAsync(SocketMessageComponent component)
    {
        if (component.Data.CustomId == "delete_message")
        {
            try
            {
                await component.Message.DeleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to delete message via button.");
            }
        }
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (IsDisabled || _config.DisableDiscord) return;
        if (message.Author.IsBot) return;

        var content = message.Content.Trim().ToLower();

        if (content == "!rebuild set-channel")
        {
            OverrideTextChannelId = message.Channel.Id;
            _statusMessageId = null; // Reset status message ID for new channel
            SaveState();
            await ReplyWithDeleteButtonAsync(message, $"ステータス表示チャンネルを <#{message.Channel.Id}> に設定しました。");
            return;
        }

        if (message.Channel.Id != (OverrideTextChannelId ?? _config.TextChannelId)) return;

        if (content == "!rebuild notify off")
        {
            IsMessageDisabled = true;
            await ReplyWithDeleteButtonAsync(message, "Discord通知を無効にしました。");
        }
        else if (content == "!rebuild notify on")
        {
            IsMessageDisabled = false;
            await ReplyWithDeleteButtonAsync(message, "Discord通知を有効にしました。");
        }
        else if (content.StartsWith("!rebuild status "))
        {
            var code = content.Substring("!rebuild status ".Length).Trim().ToUpper();
            if (code == "AUTO")
            {
                TargetRoomCode = null;
                await ReplyWithDeleteButtonAsync(message, "部屋状況の追跡を AUTO に設定しました。");
            }
            else
            {
                TargetRoomCode = code;
                await ReplyWithDeleteButtonAsync(message, $"部屋状況の追跡を `{code}` に設定しました。");
            }
        }
        else if (content == "!rebuild help")
        {
            var embed = new EmbedBuilder()
                .WithTitle("RebuildUs Bot ヘルプ")
                .WithDescription("以下のコマンドが利用可能です。")
                .AddField("!rebuild help", "このヘルプを表示します。")
                .AddField("!rebuild set-channel", "現在のチャンネルをステータス・通知送信先に設定します。")
                .AddField("!rebuild notify off/on", "ゲームイベント通知の無効化/有効化を切り替えます。")
                .AddField("!rebuild status <ROOMCODE>", "特定の部屋の状況を表示します。")
                .AddField("!rebuild status auto", "最新の部屋を自動で追跡します。")
                .WithColor(Color.Green)
                .Build();
            await ReplyWithDeleteButtonAsync(message, null, embed);
        }
    }

    private async Task ReplyWithDeleteButtonAsync(SocketMessage message, string? text, Embed? embed = null)
    {
        var builder = new ComponentBuilder()
            .WithButton("メッセージを削除", "delete_message", ButtonStyle.Secondary);

        var userMention = message.Author.Mention;
        var fullText = string.IsNullOrEmpty(text) ? $"{userMention} コマンド実行結果:" : $"{userMention} {text}";

        try
        {
            await message.DeleteAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete user command message.");
        }

        await message.Channel.SendMessageAsync(fullText, embed: embed, components: builder.Build());
    }

    private async Task ReadyAsync(DiscordSocketClient client)
    {
        if (client != _clients[0]) return;

        await client.SetGameAsync("!rebuild help", type: ActivityType.Watching);
        // Slash commands removed as per request
    }

    // private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    // {
    //     var gameCodeManager = _serviceProvider.GetRequiredService<IGameCodeManager>();

    //     if (command.Data.Name == "game")
    //     {
    //         var code = command.Data.Options.First().Value.ToString() ?? string.Empty;
    //         if (gameCodeManager.IsInUse(code))
    //         {
    //             await SetActiveRoomCodeAsync(code);
    //             await command.RespondAsync($"Room {code} status: Active - Chat sync enabled");
    //         }
    //         else
    //         {
    //             await command.RespondAsync($"Room {code} does not exist on the server.");
    //         }
    //     }
    //     else if (command.Data.Name == "end")
    //     {
    //         if (gameCodeManager.AnyInUse())
    //         {
    //             await ClearActiveRoomCodeAsync();
    //             await command.RespondAsync("Room status: Ended - Chat sync disabled");
    //         }
    //         else
    //         {
    //             await command.RespondAsync("No active rooms found on the server.");
    //         }
    //     }
    // }

    private Task LogAsync(LogMessage msg)
    {
        _logger.LogInformation(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task StartAsync()
    {
        if (IsDisabled || _config.DisableDiscord) return;

        LoadState();

        if (string.IsNullOrEmpty(_config.Token))
        {
            _logger.LogWarning("Discord token is not set. Discord features will be disabled.");
            return;
        }

        // Login and start main bot
        await _clients[0].LoginAsync(TokenType.Bot, _config.Token);
        await _clients[0].StartAsync();

        // Login and start worker bots
        for (int i = 0; i < _config.WorkerTokens.Count; i++)
        {
            await _clients[i + 1].LoginAsync(TokenType.Bot, _config.WorkerTokens[i]);
            await _clients[i + 1].StartAsync();
        }

        _logger.LogInformation("Discord bot and {Count} workers started.", _config.WorkerTokens.Count);
        await SendMessageToDiscordAsync("🚀 **RebuildUs Discord Service** が起動しました。");
    }

    public async Task StopAsync()
    {
        if (IsDisabled || _config.DisableDiscord) return;

        await SendMessageToDiscordAsync("👋 **RebuildUs Discord Service** が終了しました。");

        foreach (var client in _clients)
        {
            await client.StopAsync();
        }
    }

    public async Task SetMuteAsync(ulong discordId, bool mute, bool deaf)
    {
        if (IsDisabled || _config.DisableDiscord || _clients.Count == 0) return;

        // For single mute, just use the first available client
        await InternalSetMuteAsync(_clients[0], discordId, mute, deaf);
    }

    public async Task SetMuteAllAsync(IEnumerable<ulong> discordIds, bool mute, bool deaf)
    {
        if (IsDisabled || _config.DisableDiscord || _clients.Count == 0) return;

        var idList = discordIds.ToList();
        if (idList.Count == 0) return;

        // Distribute mute tasks among all clients to avoid rate limits
        var tasks = new List<Task>();
        for (int i = 0; i < idList.Count; i++)
        {
            var client = _clients[i % _clients.Count];
            tasks.Add(InternalSetMuteAsync(client, idList[i], mute, deaf));
        }

        await Task.WhenAll(tasks);
    }

    public async Task SendMessageToDiscordAsync(string message)
    {
        if (IsDisabled || IsMessageDisabled || _config.DisableDiscord || _clients.Count == 0) return;
        var channelId = _config.TextChannelId;
        if (channelId == 0) return;
        var client = _clients[0];
        if (client.ConnectionState != ConnectionState.Connected) return;

        var channel = await client.GetChannelAsync(channelId) as ITextChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync(message);
        }
    }

    public async Task UpdateStatusAsync(string roomCode, int playerCount, int maxPlayers, string mapName, string gameState, string playerLinkStatus)
    {
        var channelId = OverrideTextChannelId ?? _config.TextChannelId;
        if (IsDisabled || _config.DisableDiscord || _clients.Count == 0 || channelId == 0) return;

        // Auto-set if in auto mode and target is not set
        if (_isAutoTrack && string.IsNullOrEmpty(_targetRoomCode))
        {
            _targetRoomCode = roomCode.ToUpper();
        }

        if (roomCode.ToUpper() != _targetRoomCode) return;

        var client = _clients[0];
        if (client.ConnectionState != ConnectionState.Connected) return;

        var channel = await client.GetChannelAsync(channelId) as ITextChannel;
        if (channel == null) return;

        var embed = new EmbedBuilder()
            .WithTitle("Among Us 部屋状況")
            .WithColor(Color.Blue)
            .AddField("部屋コード", $"`{roomCode}`", true)
            .AddField("人数", $"{playerCount} / {maxPlayers}", true)
            .AddField("マップ", mapName, true)
            .AddField("状況", gameState, true)
            .AddField("連携状況", playerLinkStatus)
            .WithFooter("RebuildUs Status Update")
            .WithCurrentTimestamp()
            .Build();

        if (_statusMessageId.HasValue)
        {
            try
            {
                var message = await channel.GetMessageAsync(_statusMessageId.Value) as IUserMessage;
                if (message != null)
                {
                    await message.ModifyAsync(x => x.Embed = embed);
                    if (_isAutoTrack && gameState == "終了") _targetRoomCode = null;
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update status message, sending a new one.");
            }
        }

        var newMessage = await channel.SendMessageAsync(embed: embed);
        _statusMessageId = newMessage.Id;
        if (_isAutoTrack && gameState == "終了") _targetRoomCode = null;
    }

    public Task SetActiveRoomCodeAsync(string roomCode)
    {
        if (IsDisabled || _config.DisableDiscord) return Task.CompletedTask;

        _activeRoomCode = roomCode;
        _logger.LogInformation("Chat sync enabled for room {RoomCode}", roomCode);
        return Task.CompletedTask;
    }

    public Task ClearActiveRoomCodeAsync()
    {
        if (IsDisabled || _config.DisableDiscord) return Task.CompletedTask;

        _activeRoomCode = null;
        _logger.LogInformation("Chat sync disabled");
        return Task.CompletedTask;
    }

    private async Task InternalSetMuteAsync(DiscordSocketClient client, ulong discordId, bool mute, bool deaf)
    {
        if (client.ConnectionState != ConnectionState.Connected) return;

        var guild = client.GetGuild(_config.GuildId);
        if (guild == null) return;

        var user = guild.GetUser(discordId);
        if (user == null || user.VoiceChannel == null) return;

        try
        {
            if (user.IsMuted == mute && user.IsDeafened == deaf) return; // Already in desired state
            await user.ModifyAsync(x =>
            {
                x.Mute = mute;
                x.Deaf = deaf;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set mute/deaf for user {DiscordId} using client {BotUser}", discordId, client.CurrentUser?.Username);
        }
    }

    private class ServiceState
    {
        public ulong? OverrideTextChannelId { get; set; }
    }

    private void SaveState()
    {
        try
        {
            var state = new ServiceState { OverrideTextChannelId = OverrideTextChannelId };
            var json = JsonSerializer.Serialize(state);
            File.WriteAllText("rebuildus.state.json", json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to save Discord service state.");
        }
    }

    private void LoadState()
    {
        try
        {
            if (File.Exists("rebuildus.state.json"))
            {
                var json = File.ReadAllText("rebuildus.state.json");
                var state = JsonSerializer.Deserialize<ServiceState>(json);
                if (state != null)
                {
                    OverrideTextChannelId = state.OverrideTextChannelId;
                    _logger.LogInformation("Loaded Discord service state. OverrideTextChannelId: {ChannelId}", OverrideTextChannelId);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load Discord service state.");
        }
    }
}
