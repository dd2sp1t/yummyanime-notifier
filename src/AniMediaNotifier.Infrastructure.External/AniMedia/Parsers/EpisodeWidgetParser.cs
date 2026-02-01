using AniMediaNotifier.Application.AniMedia.Exceptions;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.AniMedia.Parsers.Models;
using HtmlAgilityPack;

namespace AniMediaNotifier.Infrastructure.External.AniMedia.Parsers;

internal class EpisodeWidgetParser : IEpisodeWidgetParser
{
    public ParsedEpisodeInfo[] Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var root = doc.DocumentNode;

        var seriesNodes = root
            .SelectNodes("//div[contains(@class,'pos_ser')]/a[@class='ftop-itempl d-flex has-overlay']");
        if (seriesNodes == null || seriesNodes.Count == 0)
        {
            throw new EpisodeWidgetParseException("No new episodes found in the widget");
        }

        var result = new List<ParsedEpisodeInfo>();

        foreach (var node in seriesNodes)
        {
            var ruName = GetRuName(node);
            var episodeNumber = GetEpisodeNumber(node);

            result.Add(new ParsedEpisodeInfo(ruName, episodeNumber));
        }

        return [.. result];
    }

    private static string GetRuName(HtmlNode root)
    {
        var text = root
            .SelectSingleNode(".//div[contains(@class,'ftop-item__titlepl')]")
            ?.InnerText
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EpisodeWidgetParseException("Failed to parse AnimeRuName");
        }

        return text;
    }

    private static int GetEpisodeNumber(HtmlNode root)
    {
        var text = root
            .SelectSingleNode(".//div[contains(@class,'animseripl')]/span")
            ?.InnerText
            .Trim();

        if (int.TryParse(text, out var value) == false)
        {
            throw new EpisodeWidgetParseException("Failed to parse EpisodeNumber");
        }

        return value;
    }
}