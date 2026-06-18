using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class SubscriptionRepository : ISubscriptionRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public SubscriptionRepository(YummyAnimeDbContext dbContext)
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
            dbSub.Id,
            dbSub.CreatedAt,
            dbSub.UpdatedAt,
            dbSub.UserId,
            dbSub.AnimeId,
            dbSub.TranslationSourceId,
            dbSub.IsDeleted);
    }

    public async Task<Subscription[]> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Where(s => s.UserId == userId && s.IsDeleted == false)
            .Select(s => Subscription.FromExisting(
                s.Id,
                s.CreatedAt,
                s.UpdatedAt,
                s.UserId,
                s.AnimeId,
                s.TranslationSourceId,
                s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public async Task<Subscription[]> FindByAnimeIdAsync(
        Guid animeId,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _dbContext.Subscriptions
            .Where(s => s.AnimeId == animeId && s.IsDeleted == false)
            .Select(s => Subscription.FromExisting(
                s.Id,
                s.CreatedAt,
                s.UpdatedAt,
                s.UserId,
                s.AnimeId,
                s.TranslationSourceId,
                s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public void Add(Subscription subscription)
    {
        var dbSub = new DbSubscription
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            AnimeId = subscription.AnimeId,
            TranslationSourceId = subscription.TranslationSourceId,
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
            .Select(s => Subscription.FromExisting(
                s.Id,
                s.CreatedAt,
                s.UpdatedAt,
                s.UserId,
                s.AnimeId,
                s.TranslationSourceId,
                s.IsDeleted))
            .ToArrayAsync(cancellationToken);

        return subscriptions;
    }

    public async Task UpdateRangeAsync(Subscription[] subscriptions, CancellationToken cancellationToken)
    {
        if (subscriptions.Length == 0)
        {
            return;
        }

        var subIds = subscriptions.Select(s => s.Id).ToArray();
        var dbSubscriptions = await _dbContext.Subscriptions
            .Where(db => subIds.Contains(db.Id))
            .ToArrayAsync(cancellationToken);

        var domainByKey = subscriptions.ToDictionary(keySelector: s => s.Id);

        foreach (var dbSub in dbSubscriptions)
        {
            if (domainByKey.TryGetValue(dbSub.Id, out var domainSub))
            {
                dbSub.IsDeleted = domainSub.IsDeleted;
                dbSub.UpdatedAt = domainSub.UpdatedAt;
            }
        }
    }

    public Task CancelByAnimeIdAsync(Guid animeId, CancellationToken cancellationToken)
    {
        return _dbContext.Subscriptions
            .Where(s => s.AnimeId == animeId && s.IsDeleted == false)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(s => s.IsDeleted, true)
                    .SetProperty(s => s.UpdatedAt, DateTimeOffset.UtcNow),
                cancellationToken);
    }
}