using System.Text.RegularExpressions;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Enums;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters.Models;

namespace AniMediaNotifier.Infrastructure.External.Notifications.Formatters;

internal sealed class MarkdownNotificationFormatter : INotificationFormatter
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
            ParseMode: NotificationParseMode.Markdown);
    }

    private static string EscapeMarkdownV2(string text)
    {
        return Regex.Replace(
            text,
            @"([_*\[\]()~`>#+\-=|{}.!])",
            @"\$1");
    }
}
