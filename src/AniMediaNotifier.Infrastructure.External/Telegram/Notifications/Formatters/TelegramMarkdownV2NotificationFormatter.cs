using System.Text.RegularExpressions;
using AniMediaNotifier.Application.Notifications.Formatters;
using AniMediaNotifier.Application.Notifications.Formatters.Enums;
using AniMediaNotifier.Application.Notifications.Formatters.Models;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Infrastructure.External.Telegram.Notifications.Formatters;

internal sealed class TelegramMarkdownV2NotificationFormatter : INotificationFormatter
{
    public FormattedMessage Format(Notification notification)
    {
        var title = EscapeMarkdownV2(notification.RuName);
        var url = EscapeMarkdownV2(notification.Url);

        var episodePart = notification.TotalEpisodes.HasValue
            ? $"{notification.EpisodeNumber}/{notification.TotalEpisodes}"
            : $"{notification.EpisodeNumber}/??";

        var text =
$"""
*{title}*
Вышла серия: *{episodePart}*

Ссылка:
`{url}`
""";

        return new FormattedMessage(
            Text: text,
            Format: NotificationFormat.MarkdownV2);
    }

    private static string EscapeMarkdownV2(string text)
    {
        return Regex.Replace(
            text,
            @"([_*\[\]()~`>#+\-=|{}.!])",
            @"\$1");
    }
}
