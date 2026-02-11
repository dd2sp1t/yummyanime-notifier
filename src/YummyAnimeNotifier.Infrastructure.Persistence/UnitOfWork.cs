using YummyAnimeNotifier.Application.Persistence;

namespace YummyAnimeNotifier.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly YummyAnimeDbContext _dbContext;

    public UnitOfWork(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}