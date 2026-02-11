using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface INotificationRepository
{
    Task<Notification> FindAsync(
        Guid userId,
        Guid releaseId,
        int episodeNumber,
        string translationSourceName,
        CancellationToken cancellationToken = default);

    void Add(Notification notification);

    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}