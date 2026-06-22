using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Consumer.Notifications.Formatters;

public interface INotificationFormatter
{
    FormattedMessage Format(Notification notification);
}