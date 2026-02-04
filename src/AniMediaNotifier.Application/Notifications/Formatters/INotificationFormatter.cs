using AniMediaNotifier.Application.Notifications.Formatters.Models;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Notifications.Formatters;

public interface INotificationFormatter
{
    FormattedMessage Format(Notification notification);
}