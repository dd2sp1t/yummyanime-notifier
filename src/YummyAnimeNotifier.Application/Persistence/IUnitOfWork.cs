namespace YummyAnimeNotifier.Application.Persistence;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task SaveChangesIgnoringConflictsAsync(CancellationToken cancellationToken = default);
}