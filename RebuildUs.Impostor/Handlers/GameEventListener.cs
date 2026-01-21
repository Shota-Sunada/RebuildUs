using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using RebuildUs.Impostor.Services;
using Microsoft.Extensions.Logging;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace RebuildUs.Impostor.Handlers;

public class GameEventListener : IEventListener
{
    private readonly IGameCodeManager gameCodeManager;
    private readonly IDiscordService discordService;
    private readonly IPlayerMappingService mappingService;
    private readonly ILogger<GameEventListener> logger;
    private IGame? _currentGame;
    private bool _isChatSyncActive;

    public GameEventListener(IGameCodeManager gameCodeManager, IDiscordService discordService, IPlayerMappingService mappingService, ILogger<GameEventListener> logger)
    {
        this.gameCodeManager = gameCodeManager;
        this.discordService = discordService;
        this.mappingService = mappingService;
        this.logger = logger;
    }

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameCreated(IGameCreationEvent e)
    {
        e.GameCode = gameCodeManager.Get();
        await discordService.SetActiveRoomCodeAsync(e.GameCode);
        _isChatSyncActive = true;
        await discordService.SendMessageToDiscordAsync("# 新しい部屋が建てられました。ルームコード");
        await discordService.SendMessageToDiscordAsync($"# {e.GameCode}");
    }

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameDestroyed(IGameDestroyedEvent e)
    {
        gameCodeManager.Release(e.Game.Code);
        await discordService.ClearActiveRoomCodeAsync();
        _isChatSyncActive = false;
        _currentGame = null;
        await discordService.SendMessageToDiscordAsync($"ルーム『{e.Game.Code}』は閉じられました");
    }

    [EventListener]
    public async ValueTask OnGameStarting(IGameStartingEvent e)
    {
        _currentGame = e.Game;
        await discordService.SendMessageToDiscordAsync("ゲームが開始しました。チャット同期を停止します。");
        await discordService.ClearActiveRoomCodeAsync();
        _isChatSyncActive = false;
        await MuteAllAsync(e.Game, true);
    }

    [EventListener]
    public async ValueTask OnGameEnded(IGameEndedEvent e)
    {
        await discordService.SetActiveRoomCodeAsync(e.Game.Code);
        _isChatSyncActive = true;
        await discordService.SendMessageToDiscordAsync("ゲームが終了しました。チャット同期を再開します。");
        await MuteAllAsync(e.Game, false);
    }

    [EventListener]
    public async ValueTask OnMeetingStarted(IMeetingStartedEvent e)
    {
        foreach (var player in e.Game.Players)
        {
            var friendCode = GetFriendCode(player);
            if (string.IsNullOrEmpty(friendCode)) continue;

            var discordId = mappingService.GetDiscordId(friendCode);
            if (discordId.HasValue)
            {
                await discordService.SetMuteAsync(discordId.Value, player.Character?.PlayerInfo?.IsDead ?? false);
            }
        }
    }

    [EventListener]
    public async ValueTask OnMeetingEnded(IMeetingEndedEvent e)
    {
        await MuteAllAsync(e.Game, true);
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
                mappingService.Map(friendCode, discordId);
                await e.PlayerControl.SendChatAsync($"Discord ID {discordId} has been linked to your Friend Code.");
                logger.LogInformation("Linked FriendCode {FriendCode} to Discord ID {DiscordId}", friendCode, discordId);
            }
            else
            {
                await e.PlayerControl.SendChatAsync("Usage: /link <DiscordId>");
            }
        }

        // Send game chat to Discord if sync is active
        if (_isChatSyncActive)
        {
            var playerName = GetFriendCode(e.PlayerControl);
            await discordService.SendMessageToDiscordAsync($"[{playerName}] {e.Message}");
        }
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

    private async Task MuteAllAsync(IGame game, bool mute)
    {
        foreach (var player in game.Players)
        {
            var friendCode = GetFriendCode(player);
            if (!string.IsNullOrEmpty(friendCode))
            {
                var discordId = mappingService.GetDiscordId(friendCode);
                if (discordId.HasValue)
                {
                    await discordService.SetMuteAsync(discordId.Value, mute);
                }
            }
        }
    }
}
