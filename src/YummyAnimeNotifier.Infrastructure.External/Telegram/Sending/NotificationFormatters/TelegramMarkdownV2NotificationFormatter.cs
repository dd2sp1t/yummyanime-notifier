using System.Text.RegularExpressions;
using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters;
using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters.Enums;
using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Sending.NotificationFormatters;

internal sealed class TelegramMarkdownV2NotificationFormatter : INotificationFormatter
{
    public FormattedMessage Format(Notification notification)
    {
        var title = EscapeMarkdownV2(notification.AnimeName);
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
