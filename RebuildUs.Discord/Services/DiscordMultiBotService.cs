using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmongUs.GameOptions;
using BepInEx.Unity.IL2CPP;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using InnerNet;

namespace RebuildUs.Discord.Services;

public class DiscordMultiBotService : IDisposable
{
    public static DiscordMultiBotService Instance { get; } = new();

    private DiscordSocketClient _mainClient = null!;
    private DiscordRestClient _aux1Client = null!;
    private DiscordRestClient _aux2Client = null!;

    private bool _isInitialized;
    private ulong _updateMessageId;

    // ラウンドロビン用のインデックス
    private int _botIndex;

    private DiscordMultiBotService() { }

    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        var tokenMain = Plugin.Instance.BotToken.Value;
        var tokenAux1 = Plugin.Instance.BotTokenAux1.Value;
        var tokenAux2 = Plugin.Instance.BotTokenAux2.Value;

        if (string.IsNullOrEmpty(tokenMain)) return;

        var config = new DiscordSocketConfig { GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildVoiceStates };
        var restConfig = new DiscordRestConfig();

        _mainClient = new DiscordSocketClient(config);
        _aux1Client = new DiscordRestClient(restConfig);
        _aux2Client = new DiscordRestClient(restConfig);

        _mainClient.Log += LogAsync;
        _mainClient.Ready += OnReadyAsync;
        _mainClient.SelectMenuExecuted += OnSelectMenuExecuted;

        await _mainClient.LoginAsync(TokenType.Bot, tokenMain);
        await _mainClient.StartAsync();

        if (!string.IsNullOrEmpty(tokenAux1)) await _aux1Client.LoginAsync(TokenType.Bot, tokenAux1);
        if (!string.IsNullOrEmpty(tokenAux2)) await _aux2Client.LoginAsync(TokenType.Bot, tokenAux2);

        _isInitialized = true;
    }

    private Task LogAsync(LogMessage arg)
    {
        Plugin.Instance.Log.LogInfo($"[Discord] {arg.Message} {arg.Exception}");
        return Task.CompletedTask;
    }

    private async Task OnReadyAsync()
    {
        Plugin.Instance.Log.LogInfo("[Discord] Main bot is ready!");
        await UpdateLobbyEmbedAsync();
    }

    private async Task OnSelectMenuExecuted(SocketMessageComponent component)
    {
        if (component.Data.CustomId == "link_player_select")
        {
            var selectedPlayer = component.Data.Values.FirstOrDefault();
            if (!string.IsNullOrEmpty(selectedPlayer))
            {
                LinkManager.UpdateLink(selectedPlayer, component.User.Id);
                await component.RespondAsync($"You have successfully linked your Discord account to **{selectedPlayer}**!", ephemeral: true);

                // Embedの更新
                await UpdateLobbyEmbedAsync();
            }
        }
    }

    public async Task UpdateLobbyEmbedAsync()
    {
        if (!_isInitialized) return;

        var guildIdStr = Plugin.Instance.GuildId.Value;
        var channelIdStr = Plugin.Instance.LinkChannelId.Value;
        if (!ulong.TryParse(channelIdStr, out var channelId)) return;

        var channel = _mainClient.GetChannel(channelId) as IMessageChannel;
        if (channel == null) return;

        var players = new List<string>();
        if (GameData.Instance != null && GameData.Instance.AllPlayers != null)
        {
            foreach (var pc in GameData.Instance.AllPlayers)
            {
                if (pc != null && !pc.Disconnected)
                {
                    players.Add(pc.PlayerName);
                }
            }
        }

        var embed = new EmbedBuilder()
            .WithTitle("Among Us Lobby Status")
            .WithColor(Color.Blue)
            .WithDescription(players.Count > 0 ? "Current Players in Lobby:" : "No players in lobby.")
            .WithTimestamp(DateTimeOffset.Now);

        var mapName = "Unknown";
        if (GameOptionsManager.Instance != null && GameOptionsManager.Instance.CurrentGameOptions != null)
        {
            var options = GameOptionsManager.Instance.CurrentGameOptions.Cast<NormalGameOptionsV07>();
            mapName = options.MapId.ToString();
        }

        var roomCode = AmongUsClient.Instance != null && AmongUsClient.Instance.NetworkMode == NetworkModes.OnlineGame
            ? InnerNet.GameCode.IntToGameName(AmongUsClient.Instance.GameId)
            : "Offline / Menu";

        var maxPlayers = GameOptionsManager.Instance != null && GameOptionsManager.Instance.CurrentGameOptions != null
            ? GameOptionsManager.Instance.CurrentGameOptions.MaxPlayers
            : 15;

        var hostName = "Unknown";
        if (GameData.Instance != null && AmongUsClient.Instance != null && AmongUsClient.Instance.HostId != -1)
        {
            var hostClient = GameData.Instance.GetPlayerById((byte)AmongUsClient.Instance.HostId);
            if (hostClient != null)
            {
                hostName = hostClient.PlayerName;
            }
        }

        embed.AddField("Room Code", roomCode, inline: true);
        embed.AddField("Map", mapName, inline: true);
        embed.AddField("Host", hostName, inline: true);
        embed.AddField("Players", $"{players.Count}/{maxPlayers}", inline: true);

        embed.WithDescription(players.Count > 0 ? "**Current Players:**" : "No players in lobby.");

        var selectMenu = new SelectMenuBuilder()
            .WithPlaceholder("Select your In-Game Name to link")
            .WithCustomId("link_player_select");

        foreach (var p in players)
        {
            var linkedStatus = LinkManager.TryGetDiscordId(p, out var uid) ? $" (<@{uid}>)" : " (Unlinked)";
            embed.AddField(p, $"Status: {linkedStatus}", inline: true);
            selectMenu.AddOption(p, p);
        }

        if (players.Count == 0)
        {
            selectMenu.AddOption("No players", "none");
            selectMenu.IsDisabled = true;
        }

        var component = new ComponentBuilder().WithSelectMenu(selectMenu).Build();

        if (_updateMessageId == 0)
        {
            var msg = await channel.SendMessageAsync(embed: embed.Build(), components: component);
            _updateMessageId = msg.Id;
        }
        else
        {
            try
            {
                var msg = await channel.GetMessageAsync(_updateMessageId) as IUserMessage;
                if (msg != null)
                {
                    await msg.ModifyAsync(x =>
                    {
                        x.Embed = embed.Build();
                        x.Components = component;
                    });
                }
                else
                {
                    var newMsg = await channel.SendMessageAsync(embed: embed.Build(), components: component);
                    _updateMessageId = newMsg.Id;
                }
            }
            catch
            {
                var newMsg = await channel.SendMessageAsync(embed: embed.Build(), components: component);
                _updateMessageId = newMsg.Id;
            }
        }
    }

    public struct MuteRequest
    {
        public ulong DiscordId;
        public bool Mute;
        public bool Deafen;
    }

    public async Task ApplyMuteStatesAsync(List<MuteRequest> requests)
    {
        if (!_isInitialized || !ulong.TryParse(Plugin.Instance.GuildId.Value, out var guildId)) return;

        // ボットを均等に使うためにラウンドロビンで処理
        var tasks = new List<Task>();
        var clients = new List<BaseDiscordClient> { _mainClient };
        if (_aux1Client.LoginState == LoginState.LoggedIn) clients.Add(_aux1Client);
        if (_aux2Client.LoginState == LoginState.LoggedIn) clients.Add(_aux2Client);

        foreach (var req in requests)
        {
            var client = clients[_botIndex % clients.Count];
            _botIndex++;

            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    IGuild? guild = client switch
                    {
                        DiscordSocketClient sc => sc.GetGuild(guildId),
                        DiscordRestClient rc => await rc.GetGuildAsync(guildId),
                        _ => null
                    };

                    if (guild == null) return;
                    var user = await guild.GetUserAsync(req.DiscordId);
                    if (user != null && user.VoiceChannel != null)
                    {
                        await user.ModifyAsync(x =>
                        {
                            x.Mute = req.Mute;
                            x.Deaf = req.Deafen;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Instance.Log.LogError($"[Discord] Failed to mute/deafen {req.DiscordId}: {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        _mainClient?.Dispose();
        _aux1Client?.Dispose();
        _aux2Client?.Dispose();
    }
}
