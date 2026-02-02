using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using MediatR;

namespace AniMediaNotifier.Application.Anime.Commands.UpdateAnime;

public class UpdateAnimeHandler : IRequestHandler<UpdateAnimeCommand, Unit>
{
    private readonly IAnimeRepository _animeRepository;

    public UpdateAnimeHandler(IAnimeRepository animeRepository)
    {
        _animeRepository = animeRepository;
    }

    public async Task<Unit> Handle(UpdateAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var updated = anime.TryUpdateReleasedEpisode(request.EpisodeNumber);

        if (updated == false)
        {
            return Unit.Value;
        }

        if (anime.IsFinished)
        {
            var outboxMessage = OutboxMessage.Create(new AnimeFinishedEvent(anime.Id));
            // TODO: unit of work
            await _animeRepository.UpdateWithOutboxMessageAsync(anime, outboxMessage, cancellationToken);
        }
        else
        {
            await _animeRepository.UpdateAsync(anime, cancellationToken);
        }

        return Unit.Value;
    }
}