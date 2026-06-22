using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters.Enums;

namespace YummyAnimeNotifier.Application.Consumer.Notifications.Formatters.Models;

public sealed record FormattedMessage(
    string Text,
    NotificationFormat Format);