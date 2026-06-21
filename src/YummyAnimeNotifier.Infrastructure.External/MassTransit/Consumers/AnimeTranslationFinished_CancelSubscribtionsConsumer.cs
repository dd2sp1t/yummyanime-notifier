using YummyAnimeNotifier.Application.Events;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using YummyAnimeNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

namespace YummyAnimeNotifier.Infrastructure.External.MassTransit.Consumers;

public class AnimeTranslationFinished_CancelSubscribtionsConsumer : IConsumer<AnimeTranslationFinishedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<AnimeTranslationFinished_CancelSubscribtionsConsumer> _logger;

    public AnimeTranslationFinished_CancelSubscribtionsConsumer(
        IMediator mediator,
        ILogger<AnimeTranslationFinished_CancelSubscribtionsConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<AnimeTranslationFinishedEvent> context)
    {
        var @event = context.Message;

        try
        {
            var command = new CancelSubscriptionsCommand(@event.AnimeId, @event.TranslationSourceId);
            await _mediator.Send(command);
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to cancel subscriptions for anime {AnimeId} and translation source {TranslationSourceId}",
                @event.AnimeId,
                @event.TranslationSourceId);
        }
    }
}