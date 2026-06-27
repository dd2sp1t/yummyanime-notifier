using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnime;

public class UpdateAnimeHandler : IRequestHandler<UpdateAnimeCommand, Unit>
{
    private readonly IReleaseRepository _releaseRepository;
    private readonly IAnimeRepository _animeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAnimeHandler(
        IReleaseRepository releaseRepository,
        IAnimeRepository animeRepository,
        IUnitOfWork unitOfWork)
    {
        _releaseRepository = releaseRepository;
        _animeRepository = animeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateAnimeCommand request, CancellationToken cancellationToken)
    {
        var release = await _releaseRepository.GetAsync(request.ReleaseId, cancellationToken);

        var anime = await _animeRepository.GetAsync(release.AnimeId, cancellationToken);

        // TODO: fix race
        var updated = anime.TryUpdateReleasedEpisodes(release.EpisodeNumber);

        if (updated == false)
        {
            return Unit.Value;
        }

        await _animeRepository.UpdateAsync(anime, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}