using YummyAnimeNotifier.Application.Events;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnimeTranslation;

namespace YummyAnimeNotifier.Infrastructure.External.MassTransit.Consumers;

public class ReleaseCreated_UpdateAnimeTranslationConsumer : IConsumer<ReleaseCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReleaseCreated_UpdateAnimeTranslationConsumer> _logger;

    public ReleaseCreated_UpdateAnimeTranslationConsumer(
        IMediator mediator,
        ILogger<ReleaseCreated_UpdateAnimeTranslationConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReleaseCreatedEvent> context)
    {
        var @event = context.Message;

        try
        {
            await _mediator.Send(new UpdateAnimeTranslationCommand(@event.ReleaseId));
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to update anime translation to match the release {ReleaseId}",
                @event.ReleaseId);

            throw;
        }
    }
}