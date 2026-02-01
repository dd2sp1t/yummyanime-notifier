using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly AniMediaDbContext _dbContext;

    public UserRepository(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetOrCreateByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        var dbUser = await _dbContext.Users
            .Include(u => u.Subscriptions)
            .SingleOrDefaultAsync(u => u.TelegramUserId == telegramUserId, cancellationToken);

        if (dbUser == null)
        {
            dbUser = new DbUser
            {
                Id = Guid.NewGuid(),
                TelegramUserId = telegramUserId,
                CreatedAt = DateTimeOffset.UtcNow,
                Subscriptions = []
            };
            _dbContext.Users.Add(dbUser);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return User.FromExisting(dbUser.Id, dbUser.TelegramUserId);
    }

    public async Task<User> FindByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        var dbUser = await _dbContext.Users
            .Include(u => u.Subscriptions)
            .SingleAsync(u => u.TelegramUserId == telegramUserId, cancellationToken);

        if (dbUser is null)
        {
            return null;
        }

        return User.FromExisting(dbUser.Id, dbUser.TelegramUserId);
    }
}