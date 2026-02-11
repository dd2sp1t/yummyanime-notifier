using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public UserRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetOrCreateByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
    {
        var dbUser = await _dbContext.Users
            .AsNoTracking()
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
            .AsNoTracking()
            .Include(u => u.Subscriptions)
            .SingleAsync(u => u.TelegramUserId == telegramUserId, cancellationToken);

        if (dbUser is null)
        {
            return null;
        }

        return User.FromExisting(dbUser.Id, dbUser.TelegramUserId);
    }

    public async Task<User> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbUser = await _dbContext.Users
            .AsNoTracking()
            .Include(u => u.Subscriptions)
            .SingleAsync(u => u.Id == id, cancellationToken);

        if (dbUser is null)
        {
            return null;
        }

        return User.FromExisting(dbUser.Id, dbUser.TelegramUserId);
    }
}