using Impostor.Api.Events;
using Impostor.Api.Events.Managers;
using Impostor.Api.Games;

namespace RebuildUs.Impostor.Handlers;

public class GameEventListener : IEventListener
{
    private readonly IGameCodeManager GameCodeManager;

    public GameEventListener(IGameCodeManager gameCodeManager)
    {
        GameCodeManager = gameCodeManager;
    }

    [EventListener(EventPriority.Highest)]
    public ValueTask OnGameCreated(IGameCreationEvent e)
    {
        e.GameCode = GameCodeManager.Get();
        return ValueTask.CompletedTask;
    }

    [EventListener(EventPriority.Highest)]
    public ValueTask OnGameDestroyed(IGameDestroyedEvent e)
    {
        GameCodeManager.Release(e.Game.Code);
        return ValueTask.CompletedTask;
    }
}
