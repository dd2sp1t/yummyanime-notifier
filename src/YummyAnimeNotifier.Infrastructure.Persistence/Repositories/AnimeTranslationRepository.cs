using Microsoft.EntityFrameworkCore;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class AnimeTranslationRepository : IAnimeTranslationRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public AnimeTranslationRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AnimeTranslation[]> FindAsync(Guid animeId, CancellationToken cancellationToken)
    {
        var translations = await _dbContext.AnimeTranslations
            .Where(t => t.AnimeId == animeId)
            .Select(t => AnimeTranslation.FromExisting(
                t.AnimeId,
                t.TranslationSourceId,
                t.CreatedAt,
                t.UpdatedAt,
                t.Status,
                t.TotalEpisodes,
                t.ReleasedEpisodes))
            .ToArrayAsync(cancellationToken);

        return translations;
    }

    public async Task<AnimeTranslation> FindAsync(
        Guid animeId,
        TranslationType translationType,
        string translationSourceName,
        CancellationToken cancellationToken)
    {
        var dbTranslation = await _dbContext.AnimeTranslations
            .AsNoTracking()
            .SingleOrDefaultAsync(
                t => t.AnimeId == animeId
                && t.TranslationSource.Type == translationType
                && t.TranslationSource.Name.ToLower() == translationSourceName.ToLower(),
                cancellationToken);

        if (dbTranslation is null)
        {
            return null;
        }

        return AnimeTranslation.FromExisting(
            dbTranslation.AnimeId,
            dbTranslation.TranslationSourceId,
            dbTranslation.CreatedAt,
            dbTranslation.UpdatedAt,
            dbTranslation.Status,
            dbTranslation.TotalEpisodes,
            dbTranslation.ReleasedEpisodes);
    }

    public async Task<AnimeTranslation> GetAsync(
        Guid animeId,
        Guid translationSourceId,
        CancellationToken cancellationToken)
    {
        var dbTranslation = await _dbContext.AnimeTranslations
            .AsNoTracking()
            .SingleAsync(
                t => t.AnimeId == animeId && t.TranslationSourceId == translationSourceId,
                cancellationToken);

        return AnimeTranslation.FromExisting(
            dbTranslation.AnimeId,
            dbTranslation.TranslationSourceId,
            dbTranslation.CreatedAt,
            dbTranslation.UpdatedAt,
            dbTranslation.Status,
            dbTranslation.TotalEpisodes,
            dbTranslation.ReleasedEpisodes);
    }

    public void Add(AnimeTranslation translation)
    {
        var dbTranslation = new DbAnimeTranslation
        {
            AnimeId = translation.AnimeId,
            TranslationSourceId = translation.TranslationSourceId,
            CreatedAt = translation.CreatedAt,
            UpdatedAt = translation.UpdatedAt,
            Status = translation.Status,
            TotalEpisodes = translation.TotalEpisodes,
            ReleasedEpisodes = translation.ReleasedEpisodes
        };
        _dbContext.AnimeTranslations.Add(dbTranslation);
    }

    public Task UpdateRangeAsync(AnimeTranslation[] translations, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<AnimeTranslation[]> FindAsync(
        string animeName,
        TranslationType translationType,
        string[] translationSourceNames,
        CancellationToken cancellationToken)
    {
        var translations = await _dbContext.AnimeTranslations
            .Where(at => at.Anime.Name == animeName
                    && at.TranslationSource.Type == translationType
                    && translationSourceNames.Contains(at.TranslationSource.Name))
            .Select(at => AnimeTranslation.FromExisting(
                at.AnimeId,
                at.TranslationSourceId,
                at.CreatedAt,
                at.UpdatedAt,
                at.Status,
                at.TotalEpisodes,
                at.ReleasedEpisodes))
            .ToArrayAsync(cancellationToken);

        return translations;
    }

    public async Task UpdateAsync(AnimeTranslation translation, CancellationToken cancellationToken)
    {
        var dbTranslation = await _dbContext.AnimeTranslations
            .SingleAsync(
                t => t.AnimeId == translation.AnimeId && t.TranslationSourceId == translation.TranslationSourceId,
                cancellationToken);

        dbTranslation.ReleasedEpisodes = translation.ReleasedEpisodes;
        dbTranslation.Status = translation.Status;
        dbTranslation.UpdatedAt = translation.UpdatedAt;
        dbTranslation.Version++;
    }
}