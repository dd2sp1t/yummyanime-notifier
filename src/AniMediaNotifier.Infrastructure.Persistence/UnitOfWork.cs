using AniMediaNotifier.Application.Persistence;

namespace AniMediaNotifier.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AniMediaDbContext _dbContext;

    public UnitOfWork(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);
}