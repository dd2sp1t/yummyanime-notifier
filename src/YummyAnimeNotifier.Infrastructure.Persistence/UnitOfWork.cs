using Microsoft.EntityFrameworkCore;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Infrastructure.Persistence.ConflictDetectors;

namespace YummyAnimeNotifier.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly YummyAnimeDbContext _dbContext;
    private readonly IConflictDetector _conflictDetector;

    public UnitOfWork(YummyAnimeDbContext dbContext, IConflictDetector conflictDetector)
    {
        _dbContext = dbContext;
        _conflictDetector = conflictDetector;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) => _dbContext.SaveChangesAsync(cancellationToken);

    public async Task SaveChangesIgnoringConflictsAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (_conflictDetector.IsUniqueViolation(exception))
        {
            ClearTracking();
        }
    }

    public void ClearTracking() => _dbContext.ChangeTracker.Clear();
}