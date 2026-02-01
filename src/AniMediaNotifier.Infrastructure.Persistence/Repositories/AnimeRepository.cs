using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

internal class AnimeRepository : IAnimeRepository
{
    private readonly AniMediaDbContext _dbContext;

    public AnimeRepository(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Anime> FindBySourceLinkAsync(string sourceLink, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Animes.SingleOrDefaultAsync(a => a.SourceLink == sourceLink, cancellationToken);

        if (dbAnime == null)
        {
            return null;
        }

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.SourceLink,
            dbAnime.OriginalName,
            dbAnime.RuName,
            dbAnime.Year,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodeCount,
            dbAnime.TotalEpisodeCount);
    }

    public async Task<Anime> FindByRuNameAsync(string ruName, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Animes.SingleOrDefaultAsync(a => a.RuName == ruName, cancellationToken);

        if (dbAnime == null)
        {
            return null;
        }

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.SourceLink,
            dbAnime.OriginalName,
            dbAnime.RuName,
            dbAnime.Year,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodeCount,
            dbAnime.TotalEpisodeCount);
    }

    public async Task<Anime> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Animes.SingleAsync(a => a.Id == id, cancellationToken);

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.SourceLink,
            dbAnime.OriginalName,
            dbAnime.RuName,
            dbAnime.Year,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodeCount,
            dbAnime.TotalEpisodeCount);
    }

    public async Task AddAsync(Anime anime, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(anime);

        var dbAnime = new DbAnime
        {
            Id = anime.Id,
            CreatedAt = anime.CreatedAt,
            SourceLink = anime.SourceLink,
            OriginalName = anime.OriginalName,
            RuName = anime.RuName,
            Year = anime.Year,
            Type = anime.Type,
            Status = anime.Status,
            ReleasedEpisodeCount = anime.ReleasedEpisodeCount,
            TotalEpisodeCount = anime.TotalEpisodeCount
        };
        _dbContext.Animes.Add(dbAnime);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Anime anime, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(anime);

        var dbAnime = await _dbContext.Animes.SingleAsync(a => a.Id == anime.Id, cancellationToken);

        dbAnime.ReleasedEpisodeCount = anime.ReleasedEpisodeCount;
        dbAnime.Status = anime.Status;
        dbAnime.UpdatedAt = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}