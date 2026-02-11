using Microsoft.EntityFrameworkCore;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class TranslationSourceRepository : ITranslationSourceRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public TranslationSourceRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TranslationSource[]> FindAsync(
        TranslationType type,
        string[] names,
        CancellationToken cancellationToken)
    {
        var sources = await _dbContext.TranslationSources
            .Where(s => s.Type == type
                        && names.Contains(s.Name))
            .Select(s => TranslationSource.FromExistings(s.Id, s.CreatedAt, s.Type, s.Name))
            .ToArrayAsync(cancellationToken);

        return sources;
    }

    public async Task<TranslationSource[]> GetAsync(Guid[] ids, CancellationToken cancellationToken)
    {
        var sources = await _dbContext.TranslationSources
            .Where(s => ids.Contains(s.Id))
            .Select(s => TranslationSource.FromExistings(s.Id, s.CreatedAt, s.Type, s.Name))
            .ToArrayAsync(cancellationToken);

        return sources;
    }

    public void AddRange(TranslationSource[] sources)
    {
        var dbSources = sources
            .Select(s => new DbTranslationSource
            {
                Id = s.Id,
                CreatedAt = s.CreatedAt,
                Type = s.Type,
                Name = s.Name
            })
            .ToArray();
        _dbContext.TranslationSources.AddRange(dbSources);
    }
}