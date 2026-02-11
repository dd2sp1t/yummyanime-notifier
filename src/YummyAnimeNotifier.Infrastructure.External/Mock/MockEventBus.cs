using System.Text.Json;
using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Infrastructure.External.Mock;

public class MockEventBus : IEventBus
{
    public Task<bool> TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : Event
    {
        Console.WriteLine($"Event {JsonSerializer.Serialize(@event)} was published");

        return Task.FromResult(true);
    }
}