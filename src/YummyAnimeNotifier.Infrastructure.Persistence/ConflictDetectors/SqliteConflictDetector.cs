using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence.ConflictDetectors;

internal sealed class SqliteConflictDetector : IConflictDetector
{
    public bool IsUniqueViolation(Exception exception)
    {
        var dbException = Unwrap(exception);

        if (dbException is SqliteException sqliteException)
        {
            return sqliteException.SqliteErrorCode == 19
                && sqliteException.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static Exception Unwrap(Exception exception)
    {
        return exception switch
        {
            DbUpdateException dbException => dbException.InnerException ?? dbException,
            _ => exception
        };
    }
}