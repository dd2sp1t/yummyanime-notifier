using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    public Task AddAsync(Notification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Notification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
        throw new NotImplementedException();
    }
}