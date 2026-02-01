using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Repositories;

public interface IUserRepository
{
    Task<User> GetOrCreateByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default);
    Task<User> FindByTelegramUserIdAsync(long telegramUserId, CancellationToken cancellationToken = default);
}