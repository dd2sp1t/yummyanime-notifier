using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Application.Events;

public interface IEventBus
{
    Task<bool> TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event;
}