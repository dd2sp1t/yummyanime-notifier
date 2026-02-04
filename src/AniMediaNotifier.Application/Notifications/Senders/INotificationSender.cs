using AniMediaNotifier.Application.Notifications.Senders.Models;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Notifications.Senders;

public interface INotificationSender
{
    Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken = default);
}