namespace YummyAnimeNotifier.Infrastructure.Persistence.ConflictDetectors;

internal interface IConflictDetector
{
    bool IsUniqueViolation(Exception exception);
}