using System.Text.Json;
using AniMediaNotifier.Application.Events;

namespace AniMediaNotifier.Infrastructure.External.Events;

public class MockEventBus : IEventBus
{
    public Task TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : Event
    {
        Console.WriteLine($"Event {JsonSerializer.Serialize(@event)} was published");

        return Task.CompletedTask;
    }
}