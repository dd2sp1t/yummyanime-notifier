using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Repositories;
using MediatR;

namespace AniMediaNotifier.Application.Anime.Commands.UpdateAnime;

public class UpdateAnimeHandler : IRequestHandler<UpdateAnimeCommand, Unit>
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IEventBus _eventBus;

    public UpdateAnimeHandler(IAnimeRepository animeRepository, IEventBus eventBus)
    {
        _animeRepository = animeRepository;
        _eventBus = eventBus;
    }

    public async Task<Unit> Handle(UpdateAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var updated = anime.TryUpdateReleasedEpisode(request.EpisodeNumber);

        if (updated == false)
        {
            return Unit.Value;
        }

        await _animeRepository.UpdateAsync(anime, cancellationToken);

        if (anime.IsFinished)
        {
            var @event = new AnimeFinishedEvent(anime.Id);

            await _eventBus.TryPublishAsync(@event, cancellationToken);
        }

        return Unit.Value;
    }
}