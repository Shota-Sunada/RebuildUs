using System.Text;
using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RebuildUs.Impostor.Models;
using RebuildUs.Impostor.Services;

namespace RebuildUs.Impostor.Handlers;

public sealed class GameEventListener : IEventListener
{
    private readonly DiscordConfig _config;
    private readonly IDiscordService _discordService;
    private readonly IGameCodeManager _gameCodeManager;
    private readonly ILogger<GameEventListener> _logger;
    private readonly IPlayerMappingService _mappingService;
    private IGame? _currentGame;
    private string _gameState = "待機中";
    private bool _isChatSyncActive;

    public GameEventListener(IGameCodeManager gameCodeManager, IDiscordService discordService, IPlayerMappingService mappingService, ILogger<GameEventListener> logger, IOptions<DiscordConfig> config)
    {
        _gameCodeManager = gameCodeManager;
        _discordService = discordService;
        _mappingService = mappingService;
        _logger = logger;
        _config = config.Value;
    }

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameCreated(IGameCreationEvent e)
    {
        e.GameCode = _gameCodeManager.Get();
        await _discordService.SetActiveRoomCodeAsync(e.GameCode);
        _isChatSyncActive = true;
        _gameState = "待機中";
        await _discordService.SendMessageToDiscordAsync("# 新しい部屋が建てられました。ルームコード");
        await _discordService.SendMessageToDiscordAsync($"# {e.GameCode}");
    }

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameDestroyed(IGameDestroyedEvent e)
    {
        _gameCodeManager.Release(e.Game.Code);
        await _discordService.ClearActiveRoomCodeAsync();
        _isChatSyncActive = false;
        _currentGame = null;
        _gameState = "終了";
        await _discordService.SendMessageToDiscordAsync($"ルーム『{e.Game.Code}』は閉じられました");
        await _discordService.UpdateStatusAsync(e.Game.Code, 0, e.Game.Options.MaxPlayers, e.Game.Options.Map.ToString(), "終了", "0 / 0 人が連携済み");
    }

    [EventListener]
    public async ValueTask OnGameStarting(IGameStartingEvent e)
    {
        _currentGame = e.Game;
        _gameState = "ゲーム中";
        await _discordService.SendMessageToDiscordAsync("ゲームが開始しました。チャット同期を停止します。");
        await _discordService.ClearActiveRoomCodeAsync();
        _isChatSyncActive = false;
        await MuteAllAsync(e.Game, true, true);
        await UpdateDiscordStatusAsync(e.Game, _gameState);
    }

    [EventListener]
    public async ValueTask OnGameEnded(IGameEndedEvent e)
    {
        if (_isChatSyncActive) return;
        _gameState = "待機中";
        await _discordService.SetActiveRoomCodeAsync(e.Game.Code);
        _isChatSyncActive = true;
        await _discordService.SendMessageToDiscordAsync("ゲームが終了しました。チャット同期を再開します。");
        await MuteAllAsync(e.Game, false, false);
        await UpdateDiscordStatusAsync(e.Game, _gameState);
    }

    [EventListener]
    public async ValueTask OnPlayerJoined(IGamePlayerJoinedEvent e)
    {
        await UpdateDiscordStatusAsync(e.Game, _gameState);
    }

    [EventListener]
    public async ValueTask OnPlayerLeft(IGamePlayerLeftEvent e)
    {
        await UpdateDiscordStatusAsync(e.Game, _gameState);
    }

    [EventListener]
    public async ValueTask OnMeetingStarted(IMeetingStartedEvent e)
    {
        _gameState = "会議中";
        await UpdateDiscordStatusAsync(e.Game, _gameState);
        if (!_config.AutoMute) return;

        foreach (var player in e.Game.Players)
        {
            var friendCode = GetFriendCode(player);
            if (string.IsNullOrEmpty(friendCode)) continue;

            var discordId = _mappingService.GetDiscordId(friendCode);
            if (discordId.HasValue)
            {
                var isDead = player.Character?.PlayerInfo?.IsDead ?? false;
                // 生存者: ミュート解除・デフン解除 | 死亡者: ミュート・デフン解除
                await _discordService.SetMuteAsync(discordId.Value, isDead, false);
            }
        }
    }

    [EventListener]
    public async ValueTask OnMeetingEnded(IMeetingEndedEvent e)
    {
        _gameState = "ゲーム中";
        await UpdateDiscordStatusAsync(e.Game, _gameState);

        if (!_config.AutoMute) return;

        await Task.Delay(TimeSpan.FromSeconds(7));

        var aliveUsers = new List<ulong>();
        var deadUsers = new List<ulong>();

        foreach (var player in e.Game.Players)
        {
            var friendCode = GetFriendCode(player);
            if (string.IsNullOrEmpty(friendCode)) continue;

            var discordId = _mappingService.GetDiscordId(friendCode);
            if (!discordId.HasValue) continue;

            if (player.Character?.PlayerInfo?.IsDead ?? false)
                deadUsers.Add(discordId.Value);
            else
                aliveUsers.Add(discordId.Value);
        }

        // 生存者をミュート・デフン
        await _discordService.SetMuteAllAsync(aliveUsers, true, true);

        // その後に死亡者のミュート・デフンを解除
        await _discordService.SetMuteAllAsync(deadUsers, false, false);
    }

    [EventListener]
    public async ValueTask OnPlayerChat(IPlayerChatEvent e)
    {
        if (e.Message.StartsWith("/link ", StringComparison.OrdinalIgnoreCase))
        {
            var parts = e.Message.Split(' ');
            if (parts.Length == 2 && ulong.TryParse(parts[1], out var discordId))
            {
                var friendCode = GetFriendCode(e.PlayerControl);
                if (string.IsNullOrEmpty(friendCode))
                {
                    await e.PlayerControl.SendChatAsync("Could not retrieve your Friend Code.");
                    return;
                }

                _mappingService.Map(friendCode, discordId);
                await e.PlayerControl.SendChatAsync($"Discord ID {discordId} has been linked to your Friend Code.");
                _logger.LogInformation("Linked FriendCode {FriendCode} to Discord ID {DiscordId}", friendCode, discordId);
                await UpdateDiscordStatusAsync(e.Game, _gameState);
            }
            else
                await e.PlayerControl.SendChatAsync("Usage: /link <DiscordId>");
        }

        // Send game chat to Discord if sync is active
        if (_isChatSyncActive)
        {
            var playerName = GetFriendCode(e.PlayerControl);
            await _discordService.SendMessageToDiscordAsync($"[{playerName}] {e.Message}");
        }
    }

    private async Task UpdateDiscordStatusAsync(IGame game, string state)
    {
        var roomCode = game.Code;
        var playerCount = game.Players.Count();
        var maxPlayers = game.Options.MaxPlayers;
        var mapName = game.Options.Map.ToString();

        var sb = new StringBuilder();
        var linkedCount = 0;
        foreach (var p in game.Players)
        {
            var playerName = p.Character?.PlayerInfo?.PlayerName;

            // Try to get name from FriendCode logic if character is null
            if (string.IsNullOrEmpty(playerName)) playerName = GetFriendCode(p);

            // Still empty? Use a fallback
            if (string.IsNullOrEmpty(playerName)) playerName = "Unknown Player";

            var fc = GetFriendCode(p);
            var discordId = !string.IsNullOrEmpty(fc) ? _mappingService.GetDiscordId(fc) : null;

            if (discordId.HasValue)
            {
                sb.AppendLine($"✅ {playerName} (<@{discordId.Value}>)");
                linkedCount++;
            }
            else
                sb.AppendLine($"❌ {playerName}");
        }

        if (playerCount == 0) sb.AppendLine("プレイヤーはいません");

        var summaryStatus = $"{linkedCount} / {playerCount} 人が連携済み\n\n{sb}";

        await _discordService.UpdateStatusAsync(roomCode, playerCount, maxPlayers, mapName, state, summaryStatus);
    }

    private string GetFriendCode(object player)
    {
        // TODO: Impostor API 1.10.4 does not support FriendCode directly.
        // As a workaround, we use PlayerName. If you use a customized Impostor or a client-mod to send FriendCode, update this.
        if (player is IClientPlayer clientPlayer)
            return clientPlayer.Character?.PlayerInfo?.PlayerName ?? string.Empty;
        if (player is IInnerPlayerControl playerControl)
            return playerControl.PlayerInfo?.PlayerName ?? string.Empty;
        return string.Empty;
    }

    private async Task MuteAllAsync(IGame game, bool mute, bool deaf)
    {
        if (!_config.AutoMute) return;

        var discordIds = new List<ulong>();
        foreach (var player in game.Players)
        {
            var friendCode = GetFriendCode(player);
            if (!string.IsNullOrEmpty(friendCode))
            {
                var discordId = _mappingService.GetDiscordId(friendCode);
                if (discordId.HasValue) discordIds.Add(discordId.Value);
            }
        }

        if (discordIds.Count > 0) await _discordService.SetMuteAllAsync(discordIds, mute, deaf);
    }
}
