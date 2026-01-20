using Impostor.Api.Events;

namespace RebuildUs.Impostor.Handlers;

public class GameEventListener(IGameCodeManager gameCodeManager) : IEventListener
{
    // NOTE: if you want to override the results of RebuildUs.Codes, register an event at a lower priority

    [EventListener(EventPriority.Highest)]
    public void OnGameCreated(IGameCreationEvent e) => e.GameCode = gameCodeManager.Get();

    [EventListener(EventPriority.Highest)]
    public void OnGameDestroyed(IGameDestroyedEvent e) => gameCodeManager.Release(e.Game.Code);
}