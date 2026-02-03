using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Models;

namespace AniMediaNotifier.Infrastructure.External.Notifications.Formatters;

public interface INotificationFormatter
{
    FormattedMessage Format(Notification notification);
}