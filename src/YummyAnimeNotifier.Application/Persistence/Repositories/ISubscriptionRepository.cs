using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface ISubscriptionRepository
{
    Task<Subscription> FindAsync(
        Guid userId,
        Guid animeId,
        Guid translationSourceId,
        CancellationToken cancellationToken = default);

    Task<Subscription[]> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<Subscription[]> FindByAnimeIdAsync(
        Guid animeId,
        CancellationToken cancellationToken = default);

    void Add(Subscription subscription);

    Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken = default);

    Task<Subscription[]> GetByAnimeId(
        Guid animeId,
        int take,
        int skip,
        CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(Subscription[] subscriptions, CancellationToken cancellationToken = default);

    Task CancelAsync(Guid animeId, Guid translationSourceId, CancellationToken cancellationToken = default);
}