using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface IAnimeTranslationRepository
{
    Task<AnimeTranslation[]> FindAsync(Guid animeId, CancellationToken cancellationToken = default);

    void Add(AnimeTranslation translation);

    Task UpdateRangeAsync(AnimeTranslation[] translations, CancellationToken cancellationToken = default);

    Task<AnimeTranslation[]> FindAsync(
        string animeName,
        TranslationType translationType,
        string[] translationSourceNames,
        CancellationToken cancellationToken = default);
}