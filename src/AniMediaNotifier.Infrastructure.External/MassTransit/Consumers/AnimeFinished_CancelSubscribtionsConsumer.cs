using AniMediaNotifier.Application.Events;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using AniMediaNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

namespace AniMediaNotifier.Infrastructure.External.MassTransit.Consumers;

public class AnimeFinished_CancelSubscribtionsConsumer : IConsumer<AnimeFinishedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<AnimeFinished_CancelSubscribtionsConsumer> _logger;

    public AnimeFinished_CancelSubscribtionsConsumer(
        IMediator mediator,
        ILogger<AnimeFinished_CancelSubscribtionsConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AnimeFinishedEvent> context)
    {
        var @event = context.Message;

        try
        {
            await _mediator.Send(new CancelSubscriptionsCommand(@event.AnimeId));
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to cancler subscriptions for anime {AnimeId}",
                @event.AnimeId);
        }
    }
}