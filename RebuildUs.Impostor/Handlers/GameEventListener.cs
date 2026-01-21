using Impostor.Api.Events;
using Impostor.Api.Events.Meeting;
using Impostor.Api.Events.Player;
using Impostor.Api.Games;
using RebuildUs.Impostor.Services;
using Microsoft.Extensions.Logging;
using Impostor.Api.Net;
using Impostor.Api.Net.Inner.Objects;

namespace RebuildUs.Impostor.Handlers;

public class GameEventListener(IGameCodeManager gameCodeManager, IDiscordService discordService, IPlayerMappingService mappingService, ILogger<GameEventListener> logger) : IEventListener
{
    // NOTE: if you want to override the results of RebuildUs.Codes, register an event at a lower priority

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameCreated(IGameCreationEvent e)
    {
        e.GameCode = gameCodeManager.Get();
        await discordService.CallGameAsync(e.GameCode);
    }

    [EventListener(EventPriority.Highest)]
    public async ValueTask OnGameDestroyed(IGameDestroyedEvent e)
    {
        gameCodeManager.Release(e.Game.Code);
        await discordService.CallEndAsync();
    }

    [EventListener]
    public async ValueTask OnGameStarting(IGameStartingEvent e)
    {
        await MuteAllAsync(e.Game, true);
    }

    [EventListener]
    public async ValueTask OnGameEnded(IGameEndedEvent e)
    {
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
