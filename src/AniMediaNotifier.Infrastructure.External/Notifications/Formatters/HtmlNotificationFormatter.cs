using System.Net;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Enums;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Models;

namespace AniMediaNotifier.Infrastructure.External.Notifications.Formatters;

internal sealed class HtmlNotificationFormatter : INotificationFormatter
{
    public FormattedMessage Format(Notification notification)
    {
        var title = HtmlEscape(notification.RuName);
        var url = HtmlEscape(notification.Url);

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
            ParseMode: NotificationParseMode.Html);
    }

    private static string HtmlEscape(string text) => WebUtility.HtmlEncode(text);
}