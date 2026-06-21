using Microsoft.EntityFrameworkCore;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class ReleaseRepository : IReleaseRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public ReleaseRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Release release)
    {
        var dbRelease = new DbRelease
        {
            Id = release.Id,
            CreatedAt = release.CreatedAt,
            AnimeId = release.AnimeId,
            TranslationSourceId = release.TranslationSourceId,
            EpisodeNumber = release.EpisodeNumber
        };
        _dbContext.Releases.Add(dbRelease);
    }

    public async Task<Release> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbRelease = await _dbContext.Releases
            .AsNoTracking()
            .SingleAsync(
                r => r.Id == id,
                cancellationToken);

        return Release.FromExisting(
            dbRelease.Id,
            dbRelease.CreatedAt,
            dbRelease.AnimeId,
            dbRelease.TranslationSourceId,
            dbRelease.EpisodeNumber
        );
    }
}