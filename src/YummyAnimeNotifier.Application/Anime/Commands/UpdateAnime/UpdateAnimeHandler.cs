using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;

namespace YummyAnimeNotifier.Application.Anime.Commands.UpdateAnime;

public class UpdateAnimeHandler : IRequestHandler<UpdateAnimeCommand, Unit>
{
    private readonly IAnimeRepository _animeRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAnimeHandler(
        IAnimeRepository animeRepository,
        IOutboxMessageRepository outboxMessageRepository,
        IUnitOfWork unitOfWork)
    {
        _animeRepository = animeRepository;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var updated = anime.TryUpdateReleasedEpisodes(request.EpisodeNumber);

        if (updated == false)
        {
            return Unit.Value;
        }

        await _animeRepository.UpdateAsync(anime, cancellationToken);

        if (anime.IsFinished)
        {
            var @event = new AnimeFinishedEvent(anime.Id);
            _outboxMessageRepository.Add(OutboxMessage.Create(@event));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}