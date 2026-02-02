using AniMediaNotifier.Domain.Events;

namespace AniMediaNotifier.Application.Events;

public interface IEventBus
{
    Task<bool> TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : Event;
}