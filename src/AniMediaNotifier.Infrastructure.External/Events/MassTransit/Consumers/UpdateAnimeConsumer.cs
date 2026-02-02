using AniMediaNotifier.Application.Events;
using MediatR;
using MassTransit;
using AniMediaNotifier.Application.Anime.Commands.UpdateAnime;
using Microsoft.Extensions.Logging;

namespace AniMediaNotifier.Infrastructure.External.Events.MassTransit.Consumers;

public class UpdateAnimeConsumer : IConsumer<NewEpisodeDetectedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UpdateAnimeConsumer> _logger;

    public UpdateAnimeConsumer(IMediator mediator, ILogger<UpdateAnimeConsumer> logger)
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