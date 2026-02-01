using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Notifications;

public interface INotificationSender
{
    Task SendAsync(Notification notification, CancellationToken cancellationToken = default);
}