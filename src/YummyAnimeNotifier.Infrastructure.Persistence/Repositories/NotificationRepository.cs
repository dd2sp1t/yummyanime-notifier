using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Repositories;

internal class NotificationRepository : INotificationRepository
{
    private readonly YummyAnimeDbContext _dbContext;

    public NotificationRepository(YummyAnimeDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Notification> FindAsync(
        Guid userId,
        Guid releaseId,
        int episodeNumber,
        string translationSourceName,
        CancellationToken cancellationToken)
    {
        var dbNotification = await _dbContext.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId
                    && n.ReleaseId == releaseId
                    && n.EpisodeNumber == episodeNumber
                    && n.TranslationSourceName == translationSourceName)
            .SingleOrDefaultAsync(cancellationToken);

        if (dbNotification is null)
        {
            return null;
        }

        return Notification.FromExisting(
            dbNotification.Id,
            dbNotification.CreatedAt,
            dbNotification.UpdatedAt,
            dbNotification.UserId,
            dbNotification.ReleaseId,
            dbNotification.AnimeName,
            dbNotification.Url,
            dbNotification.TotalEpisodes,
            dbNotification.EpisodeNumber,
            dbNotification.TranslationSourceName,
            dbNotification.Status,
            dbNotification.Error);
    }

    public void Add(Notification notification)
    {
        var dbNotification = new DbNotification
        {
            Id = notification.Id,
            CreatedAt = notification.CreatedAt,
            UpdatedAt = notification.UpdatedAt,
            UserId = notification.UserId,
            ReleaseId = notification.ReleaseId,
            AnimeName = notification.AnimeName,
            Url = notification.Url,
            TotalEpisodes = notification.TotalEpisodes,
            EpisodeNumber = notification.EpisodeNumber,
            TranslationSourceName = notification.TranslationSourceName,
            Status = notification.Status,
            Error = notification.Error
        };
        _dbContext.Notifications.Add(dbNotification);
    }

    public async Task UpdateAsync(Notification notification, CancellationToken cancellationToken)
    {
        var dbNotification = await _dbContext.Notifications.SingleAsync(
            n => n.Id == notification.Id,
            cancellationToken);

        dbNotification.Status = notification.Status;
        dbNotification.Error = notification.Error;
        dbNotification.UpdatedAt = notification.UpdatedAt;
    }
}