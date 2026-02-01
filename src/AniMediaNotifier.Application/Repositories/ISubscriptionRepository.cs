using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription> FindAsync(Guid userId, Guid animeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Subscription>> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Subscription>> FindByAnimeIdAsync(
        Guid animeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(Subscription subscription, CancellationToken cancellationToken = default);

    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);

    Task<ICollection<Subscription>> GetByAnimeId(
        Guid animeId,
        int take,
        int skip,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(ICollection<Subscription> subscriptions, CancellationToken cancellationToken = default);
}