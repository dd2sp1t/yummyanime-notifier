using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Repositories;

public interface INotificationRepository
{
    Task<Notification> FindAsync(
        Guid userId,
        Guid animeId,
        int episodeNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);

    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}