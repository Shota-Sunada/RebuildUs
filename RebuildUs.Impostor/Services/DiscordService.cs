using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace RebuildUs.Impostor.Services;

public interface IDiscordService
{
    Task StartAsync();
    Task StopAsync();
    Task SetMuteAsync(ulong discordId, bool mute);
    Task SetMuteAllAsync(IEnumerable<ulong> discordIds, bool mute);
    Task CallGameAsync(string roomCode);
    Task CallEndAsync();
}

public class DiscordService : IDiscordService
{
    private readonly List<DiscordSocketClient> _clients = [];
    private readonly Models.DiscordConfig _config;
    private readonly ILogger<DiscordService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public DiscordService(ILogger<DiscordService> logger, IOptions<Models.DiscordConfig> config, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _config = config.Value;
        _serviceProvider = serviceProvider;

        // Add main client
        _clients.Add(CreateClient());

        // Add worker clients
        foreach (var _ in _config.WorkerTokens)
        {
            _clients.Add(CreateClient());
        }
    }

    private DiscordSocketClient CreateClient()
    {
        var client = new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates | GatewayIntents.GuildMembers
        });
        client.Log += LogAsync;
        client.Ready += () => ReadyAsync(client);
        return client;
    }

    private async Task ReadyAsync(DiscordSocketClient client)
    {
        if (client != _clients[0]) return;

        client.SlashCommandExecuted += SlashCommandExecutedAsync;

        var gameCommand = new SlashCommandBuilder()
            .WithName("game")
            .WithDescription("Start a game room")
            .AddOption("code", ApplicationCommandOptionType.String, "The room code", isRequired: true);

        var endCommand = new SlashCommandBuilder()
            .WithName("end")
            .WithDescription("End the game room");

        try
        {
            await client.CreateGlobalApplicationCommandAsync(gameCommand.Build());
            await client.CreateGlobalApplicationCommandAsync(endCommand.Build());
            _logger.LogInformation("Discord slash commands registered.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to register slash commands.");
        }
    }

    private async Task SlashCommandExecutedAsync(SocketSlashCommand command)
    {
        var gameCodeManager = _serviceProvider.GetRequiredService<IGameCodeManager>();

        if (command.Data.Name == "game")
        {
            var code = command.Data.Options.First().Value.ToString() ?? string.Empty;
            if (gameCodeManager.IsInUse(code))
            {
                await command.RespondAsync($"Room {code} status: Active");
            }
            else
            {
                await command.RespondAsync($"Room {code} does not exist on the server.");
            }
        }
        else if (command.Data.Name == "end")
        {
            if (gameCodeManager.AnyInUse())
            {
                await command.RespondAsync("Room status: Ended");
            }
            else
            {
                await command.RespondAsync("No active rooms found on the server.");
            }
        }
    }

    private Task LogAsync(LogMessage msg)
    {
        _logger.LogInformation(msg.ToString());
        return Task.CompletedTask;
    }

    public async Task StartAsync()
    {
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
    }

    public async Task StopAsync()
    {
        foreach (var client in _clients)
        {
            await client.StopAsync();
        }
    }

    public async Task SetMuteAsync(ulong discordId, bool mute)
    {
        // For single mute, just use the first available client
        await InternalSetMuteAsync(_clients[0], discordId, mute);
    }

    public async Task SetMuteAllAsync(IEnumerable<ulong> discordIds, bool mute)
    {
        var idList = discordIds.ToList();
        if (idList.Count == 0) return;

        // Distribute mute tasks among all clients to avoid rate limits
        var tasks = new List<Task>();
        for (int i = 0; i < idList.Count; i++)
        {
            var client = _clients[i % _clients.Count];
            tasks.Add(InternalSetMuteAsync(client, idList[i], mute));
        }

        await Task.WhenAll(tasks);
    }

    public async Task CallGameAsync(string roomCode)
    {
        if (_config.TextChannelId == 0) return;
        var client = _clients[0];
        if (client.ConnectionState != ConnectionState.Connected) return;

        var channel = await client.GetChannelAsync(_config.TextChannelId) as ITextChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync($"/game {roomCode}");
        }
    }

    public async Task CallEndAsync()
    {
        if (_config.TextChannelId == 0) return;
        var client = _clients[0];
        if (client.ConnectionState != ConnectionState.Connected) return;

        var channel = await client.GetChannelAsync(_config.TextChannelId) as ITextChannel;
        if (channel != null)
        {
            await channel.SendMessageAsync("/end");
        }
    }

    private async Task InternalSetMuteAsync(DiscordSocketClient client, ulong discordId, bool mute)
    {
        if (client.ConnectionState != ConnectionState.Connected) return;

        var guild = client.GetGuild(_config.GuildId);
        if (guild == null) return;

        var user = guild.GetUser(discordId);
        if (user == null || user.VoiceChannel == null) return;

        try
        {
            if (user.IsMuted == mute) return; // Already in desired state
            await user.ModifyAsync(x => x.Mute = mute);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set mute for user {DiscordId} using client {BotUser}", discordId, client.CurrentUser?.Username);
        }
    }
}
