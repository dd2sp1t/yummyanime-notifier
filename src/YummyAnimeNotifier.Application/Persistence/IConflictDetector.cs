namespace YummyAnimeNotifier.Application.Persistence;

public interface IConflictDetector
{
    bool IsUniqueViolation(Exception exception);
}