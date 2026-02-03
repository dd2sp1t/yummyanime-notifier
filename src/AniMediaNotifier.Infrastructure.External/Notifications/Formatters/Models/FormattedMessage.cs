using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Enums;

namespace AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Models;

public sealed record FormattedMessage(
    string Text,
    NotificationParseMode ParseMode);