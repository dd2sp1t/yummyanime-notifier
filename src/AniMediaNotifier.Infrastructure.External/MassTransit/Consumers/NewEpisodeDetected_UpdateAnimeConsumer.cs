using AniMediaNotifier.Application.Events;
using MediatR;
using MassTransit;
using AniMediaNotifier.Application.Anime.Commands.UpdateAnime;
using Microsoft.Extensions.Logging;

namespace AniMediaNotifier.Infrastructure.External.MassTransit.Consumers;

public class NewEpisodeDetected_UpdateAnimeConsumer : IConsumer<NewEpisodeDetectedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<NewEpisodeDetected_UpdateAnimeConsumer> _logger;

    public NewEpisodeDetected_UpdateAnimeConsumer(
        IMediator mediator,
        ILogger<NewEpisodeDetected_UpdateAnimeConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewEpisodeDetectedEvent> context)
    {
        var @event = context.Message;

        try
        {
            await _mediator.Send(new UpdateAnimeCommand(@event.AnimeId, @event.EpisodeNumber));
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to update anime {AnimeId}",
                @event.AnimeId);
        }
    }
}