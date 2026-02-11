using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface IUserRepository
{
    Task<User> GetOrCreateByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default);
    Task<User> FindByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default);
    Task<User> GetAsync(Guid id, CancellationToken cancellationToken = default);
}