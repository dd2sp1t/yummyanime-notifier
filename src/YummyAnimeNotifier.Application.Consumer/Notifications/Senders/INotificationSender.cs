using YummyAnimeNotifier.Application.Consumer.Notifications.Senders.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Consumer.Notifications.Senders;

public interface INotificationSender
{
    Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken = default);
}