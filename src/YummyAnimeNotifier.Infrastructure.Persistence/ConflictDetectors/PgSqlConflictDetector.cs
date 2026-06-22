using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace YummyAnimeNotifier.Infrastructure.Persistence.ConflictDetectors;

internal sealed class PgSqlConflictDetector : IConflictDetector
{
    public bool IsUniqueViolation(Exception exception)
    {
        var dbException = Unwrap(exception);

        if (dbException is PostgresException pgException)
        {
            return pgException.SqlState == PostgresErrorCodes.UniqueViolation;
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