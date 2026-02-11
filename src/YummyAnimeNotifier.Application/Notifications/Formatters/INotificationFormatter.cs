using YummyAnimeNotifier.Application.Notifications.Formatters.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Notifications.Formatters;

public interface INotificationFormatter
{
    FormattedMessage Format(Notification notification);
}