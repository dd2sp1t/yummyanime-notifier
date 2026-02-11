using YummyAnimeNotifier.Application.Notifications.Senders.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Notifications.Senders;

public interface INotificationSender
{
    Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken = default);
}