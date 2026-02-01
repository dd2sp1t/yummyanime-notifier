namespace AniMediaNotifier.Application.Events;

public interface IEventBus
{
    Task TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : Event;
}