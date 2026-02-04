using AniMediaNotifier.Application.Notifications.Formatters.Enums;

namespace AniMediaNotifier.Application.Notifications.Formatters.Models;

public sealed record FormattedMessage(
    string Text,
    NotificationFormat Format);