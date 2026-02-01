using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Repositories;

public interface INotificationRepository
{
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
    Task UpdateAsync(Notification notification, CancellationToken cancellationToken = default);
}