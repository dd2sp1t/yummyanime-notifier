using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;
using YummyAnimeNotifier.Application.Markers;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnimeTranslation;

public class UpdateAnimeTranslationHandler :
    IRequestHandler<UpdateAnimeTranslationCommand, Unit>,
    IConsumerAssemblyMarker
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IAnimeTranslationRepository _animeTranslationRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAnimeTranslationHandler(
        IReleaseRepository releaseRepository,
        IAnimeTranslationRepository animeRepository,
        IOutboxMessageRepository outboxMessageRepository,

        IUnitOfWork unitOfWork)
    {
        _releaseRepository = releaseRepository;
        _animeTranslationRepository = animeRepository;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateAnimeTranslationCommand request, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetAsync(request.ReleaseId, cancellationToken);

        var animeTranslation = await _animeTranslationRepository.GetAsync(
            release.AnimeId,
            release.TranslationSourceId,
            cancellationToken);

        var updated = animeTranslation.TryUpdateReleasedEpisodes(release.EpisodeNumber);

        if (updated == false)
        {
            return Unit.Value;
        }

        await _animeTranslationRepository.UpdateAsync(animeTranslation, cancellationToken);

        if (animeTranslation.IsFinished)
        {
            var @event = new AnimeTranslationFinishedEvent(
                animeTranslation.AnimeId,
                animeTranslation.TranslationSourceId);
            _outboxMessageRepository.Add(OutboxMessage.Create(@event));
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}