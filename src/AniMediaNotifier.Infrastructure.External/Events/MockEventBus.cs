using System.Text.Json;
using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Domain.Events;

namespace AniMediaNotifier.Infrastructure.External.Events;

public class MockEventBus : IEventBus
{
    public Task<bool> TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : Event
    {
        Console.WriteLine($"Event {JsonSerializer.Serialize(@event)} was published");

        return Task.FromResult(true);
    }
}