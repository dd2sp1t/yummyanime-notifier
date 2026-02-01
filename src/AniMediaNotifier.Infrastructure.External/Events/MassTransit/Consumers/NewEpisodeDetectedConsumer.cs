using AniMediaNotifier.Application.Notifications.Commands.NotifyUsers;
using AniMediaNotifier.Application.Events;
using MediatR;
using MassTransit;
using AniMediaNotifier.Application.Anime.Commands.UpdateAnime;

namespace AniMediaNotifier.Infrastructure.External.Events.MassTransit.Consumers;

public class NewEpisodeDetectedConsumer : IConsumer<NewEpisodeDetectedEvent>
{
    private readonly IMediator _mediator;

    public NewEpisodeDetectedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<NewEpisodeDetectedEvent> context)
    {
        var @event = context.Message;

        // await _mediator.Send(new UpdateAnimeCommand(@event.AnimeId, @event.EpisodeNumber));

        await _mediator.Send(new NotifyUsersCommand(@event.AnimeId, @event.EpisodeNumber));
    }
}