using AniMediaNotifier.Application.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AniMediaNotifier.Infrastructure.External.Events.MassTransit;

public class MassTransitEventBus : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<MassTransitEventBus> _logger;

    public MassTransitEventBus(IPublishEndpoint publishEndpoint, ILogger<MassTransitEventBus> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task TryPublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken)
        where TEvent : Application.Events.Event
    {
        try
        {
            await _publishEndpoint.Publish(@event, cancellationToken);
        }
        catch (Exception exception)
        {
            _logger.Log(LogLevel.Error, exception, "Error while publishing event");
        }
    }
}