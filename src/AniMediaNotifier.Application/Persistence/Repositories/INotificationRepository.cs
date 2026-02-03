using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Persistence.Repositories;

public interface INotificationRepository
{
    Task<Notification> FindAsync(
        Guid userId,
        Guid animeId,
        int episodeNumber,
        CancellationToken cancellationToken = default);

    void Add(Notification notification);

    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}