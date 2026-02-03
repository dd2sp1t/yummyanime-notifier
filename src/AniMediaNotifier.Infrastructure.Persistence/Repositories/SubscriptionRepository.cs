using AniMediaNotifier.Application.Persistence.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

internal class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AniMediaDbContext _dbContext;

    public SubscriptionRepository(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Subscription> FindAsync(Guid userId, Guid animeId, CancellationToken cancellationToken)
    {
        var dbSub = await _dbContext.Subscriptions
            .AsNoTracking()
            .SingleOrDefaultAsync(
                s => s.UserId == userId && s.AnimeId == animeId,
                cancellationToken);

        if (dbSub is null)
        {
            return null;
        }

        return Subscription.FromExisting(
            dbSub.UserId,
            dbSub.AnimeId,
            dbSub.CreatedAt,
            dbSub.UpdatedAt,
            dbSub.IsDeleted);
    }

    public async Task<Subscription[]> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Where(s => s.UserId == userId && s.IsDeleted == false)
            .Select(s => Subscription.FromExisting(s.UserId, s.AnimeId, s.CreatedAt, s.UpdatedAt, s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription[]> FindByAnimeIdAsync(
        Guid animeId,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Where(s => s.AnimeId == animeId && s.IsDeleted == false)
            .Select(s => Subscription.FromExisting(s.UserId, s.AnimeId, s.CreatedAt, s.UpdatedAt, s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public void Add(Subscription subscription)
    {
        var dbSub = new DbSubscription
        {
            UserId = subscription.UserId,
            AnimeId = subscription.AnimeId,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt,
            IsDeleted = subscription.IsDeleted
        };
        _dbContext.Subscriptions.Add(dbSub);
    }

    public async Task UpdateAsync(Subscription subscription, CancellationToken cancellationToken)
    {
        var dbSub = await _dbContext.Subscriptions.SingleOrDefaultAsync(
            s => s.UserId == subscription.UserId && s.AnimeId == subscription.AnimeId,
            cancellationToken);

        dbSub.IsDeleted = subscription.IsDeleted;
        dbSub.UpdatedAt = subscription.UpdatedAt;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Subscription[]> GetByAnimeId(
        Guid animeId,
        int take,
        int skip,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Where(s => s.AnimeId == animeId && s.IsDeleted == false)
            .Skip(skip)
            .Take(take)
            .Select(s => Subscription.FromExisting(s.UserId, s.AnimeId, s.CreatedAt, s.UpdatedAt, s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public async Task UpdateRangeAsync(Subscription[] subscriptions, CancellationToken cancellationToken)
    {
        if (subscriptions.Length == 0)
        {
            return;
        }

        var domainByKey = subscriptions.ToDictionary(s => (s.UserId, s.AnimeId));

        var userIds = subscriptions.Select(s => s.UserId).Distinct().ToArray();
        var animeIds = subscriptions.Select(s => s.AnimeId).Distinct().ToArray();

        var dbSubscriptions = await _dbContext.Subscriptions
            .Where(db => userIds.Contains(db.UserId) && animeIds.Contains(db.AnimeId))
            .ToArrayAsync(cancellationToken);

        foreach (var dbSub in dbSubscriptions)
        {
            var key = (dbSub.UserId, dbSub.AnimeId);
            if (domainByKey.TryGetValue(key, out var domainSub))
            {
                dbSub.IsDeleted = domainSub.IsDeleted;
                dbSub.UpdatedAt = domainSub.UpdatedAt;
            }
        }
    }
}