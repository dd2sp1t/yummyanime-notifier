using YummyAnimeNotifier.Application.Notifications.Formatters.Enums;

namespace YummyAnimeNotifier.Application.Notifications.Formatters.Models;

public sealed record FormattedMessage(
    string Text,
    NotificationFormat Format);