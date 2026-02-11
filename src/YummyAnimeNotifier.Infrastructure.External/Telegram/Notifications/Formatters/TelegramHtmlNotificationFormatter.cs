using System.Net;
using YummyAnimeNotifier.Application.Notifications.Formatters;
using YummyAnimeNotifier.Application.Notifications.Formatters.Enums;
using YummyAnimeNotifier.Application.Notifications.Formatters.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Notifications.Formatters;

internal sealed class TelegramHtmlNotificationFormatter : INotificationFormatter
{
    public FormattedMessage Format(Notification notification)
    {
        var title = EscapeHtml(notification.AnimeName);
        var url = EscapeHtml(notification.Url);

        var episodePart = notification.TotalEpisodes.HasValue
            ? $"{notification.EpisodeNumber}/{notification.TotalEpisodes}"
            : $"{notification.EpisodeNumber}/??";

        var text =
$"""
<b>{title}</b>
Вышла серия: <b>{episodePart}</b>

Ссылка:
<code>{url}</code>
""";

        return new FormattedMessage(
            Text: text,
            Format: NotificationFormat.Html);
    }

    private static string EscapeHtml(string text) => WebUtility.HtmlEncode(text);
}