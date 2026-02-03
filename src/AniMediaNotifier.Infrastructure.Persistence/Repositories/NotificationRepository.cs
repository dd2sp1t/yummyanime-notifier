using AniMediaNotifier.Application.Persistence.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

internal class NotificationRepository : INotificationRepository
{
    private readonly AniMediaDbContext _dbContext;

    public NotificationRepository(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Notification> FindAsync(
        Guid userId,
        Guid animeId,
        int episodeNumber,
        CancellationToken cancellationToken)
    {
        var dbNotification = await _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId
                    && n.AnimeId == animeId
                    && n.EpisodeNumber == episodeNumber)
            .SingleOrDefaultAsync(cancellationToken);

        if (dbNotification is null)
        {
            return null;
        }

        return Notification.FromExisting(
            dbNotification.Id,
            dbNotification.CreatedAt,
            dbNotification.UserId,
            dbNotification.AnimeId,
            dbNotification.EpisodeNumber,
            dbNotification.Message,
            dbNotification.IsSent,
            dbNotification.SentAt);
    }

    public void Add(Notification notification)
    {
        var dbNotification = new DbNotification
        {
            Id = notification.Id,
            CreatedAt = notification.CreatedAt,
            UserId = notification.UserId,
            AnimeId = notification.AnimeId,
            EpisodeNumber = notification.EpisodeNumber,
            Message = notification.Message,
            IsSent = notification.IsSent,
            SentAt = notification.SentAt
        };
        _dbContext.Notifications.Add(dbNotification);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken)
    {
        var dbNotification = await _dbContext.Notifications.SingleAsync(
            n => n.Id == notification.Id,
            cancellationToken);

        dbNotification.IsSent = notification.IsSent;
        dbNotification.SentAt = notification.SentAt;
    }
}