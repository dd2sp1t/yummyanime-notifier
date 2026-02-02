using AniMediaNotifier.Application.Notifications.Commands.NotifyUsers;
using AniMediaNotifier.Application.Events;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AniMediaNotifier.Infrastructure.External.Events.MassTransit.Consumers;

public class NotifyUsersConsumer : IConsumer<NewEpisodeDetectedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotifyUsersConsumer> _logger;

    public NotifyUsersConsumer(IMediator mediator, ILogger<NotifyUsersConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewEpisodeDetectedEvent> context)
    {
        var @event = context.Message;

        try
        {
            await _mediator.Send(new NotifyUsersCommand(@event.AnimeId, @event.EpisodeNumber));
        }
        catch(Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to notify users about anime {AnimeId}",
                @event.AnimeId);
        }
    }
}