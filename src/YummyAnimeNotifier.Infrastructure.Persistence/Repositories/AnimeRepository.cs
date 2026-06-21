using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class AnimeRepository : IAnimeRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public AnimeRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Anime> FindBySourceLinkAsync(string sourceLink, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Anime
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.SourceLink == sourceLink, cancellationToken);

        if (dbAnime == null)
        {
            return null;
        }

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.UpdatedAt,
            dbAnime.ExternalId,
            dbAnime.SourceLink,
            dbAnime.Name,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodes,
            dbAnime.TotalEpisodes);
    }

    public async Task<Anime> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Anime
            .AsNoTracking()
            .SingleOrDefaultAsync(a => a.Name == name, cancellationToken);

        if (dbAnime == null)
        {
            return null;
        }

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.UpdatedAt,
            dbAnime.ExternalId,
            dbAnime.SourceLink,
            dbAnime.Name,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodes,
            dbAnime.TotalEpisodes);
    }

    public async Task<Anime> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Anime
            .AsNoTracking()
            .SingleAsync(a => a.Id == id, cancellationToken);

        return Anime.FromExisting(
            dbAnime.Id,
            dbAnime.CreatedAt,
            dbAnime.UpdatedAt,
            dbAnime.ExternalId,
            dbAnime.SourceLink,
            dbAnime.Name,
            dbAnime.Type,
            dbAnime.Status,
            dbAnime.ReleasedEpisodes,
            dbAnime.TotalEpisodes);
    }

    public async Task<Anime[]> GetAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        var anime = await _dbContext.Anime
            .Where(a => ids.Contains(a.Id))
            .Select(dbAnime => Anime.FromExisting(
                dbAnime.Id,
                dbAnime.CreatedAt,
                dbAnime.UpdatedAt,
                dbAnime.ExternalId,
                dbAnime.SourceLink,
                dbAnime.Name,
                dbAnime.Type,
                dbAnime.Status,
                dbAnime.ReleasedEpisodes,
                dbAnime.TotalEpisodes))
            .ToArrayAsync();

        return anime;
    }

    public void Add(Anime anime)
    {
        var dbAnime = new DbAnime
        {
            Id = anime.Id,
            CreatedAt = anime.CreatedAt,
            UpdatedAt = anime.UpdatedAt,
            ExternalId = anime.ExternalId,
            SourceLink = anime.SourceLink,
            Name = anime.Name,
            Type = anime.Type,
            Status = anime.Status,
            ReleasedEpisodes = anime.ReleasedEpisodes,
            TotalEpisodes = anime.TotalEpisodes
        };
        _dbContext.Anime.Add(dbAnime);
    }

    public async Task UpdateAsync(Anime anime, CancellationToken cancellationToken)
    {
        var dbAnime = await _dbContext.Anime.SingleAsync(a => a.Id == anime.Id, cancellationToken);

        dbAnime.ReleasedEpisodes = anime.ReleasedEpisodes;
        dbAnime.Status = anime.Status;
        dbAnime.UpdatedAt = anime.UpdatedAt;
    }

    public async Task<string[]> FilterSubscribedNamesAsync(string[] names, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Anime
            .Where(a => names.Contains(a.Name) && a.Subscriptions.Any(s => s.IsDeleted == false))
            .Select(a => a.Name)
            .ToArrayAsync(cancellationToken);

        return existing;
    }
}