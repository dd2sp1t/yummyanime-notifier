using YummyAnimeNotifier.Application.YummyAnime.Exceptions;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime.Parsers;

internal class AnimeParser : IAnimeParser
{
    private static readonly Regex _xEpisodeCountRegex;
    private static readonly Regex _xyEpisodeCountRegex;

    static AnimeParser()
    {
        // "Количество серий: X"
        _xEpisodeCountRegex = new(@"^\d+$", RegexOptions.Compiled);

        // "Вышло серий: X из Y"
        _xyEpisodeCountRegex = new(@"^(\d+)\s+из\s+(\d+)$", RegexOptions.Compiled);
    }

    public ParsedAnime Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var root = doc.DocumentNode;

        var externalId = GetExternalId(root);
        var sourceLink = GetSourceLink(root);
        var name = GetName(root);
        var typeRaw = GetType(root);
        var statusRaw = GetStatus(root);
        var totalEpisodes = GetTotalEpisodes(root);
        var releasedEpisodes = GetReleasedEpisodes(root);

        return new ParsedAnime(
            externalId,
            sourceLink,
            name,
            typeRaw,
            statusRaw,
            totalEpisodes,
            releasedEpisodes
        );
    }

    private static int GetExternalId(HtmlNode root)
    {
        var value = root
            .SelectSingleNode("//meta[@name='page_id']")
            ?.GetAttributeValue("content", null)
            ?.Trim();

        if (int.TryParse(value, out var id) == false)
        {
            throw new ParseException("Failed to parse ExternalId (page_id)");
        }

        return id;
    }

    private static string GetSourceLink(HtmlNode root)
    {
        var value = root
            .SelectSingleNode("//meta[@property='og:url']")
            ?.GetAttributeValue("content", null)
            ?.Trim();

        if (Uri.TryCreate(value, UriKind.Relative, out var _) == false)
        {
            throw new ParseException("Failed to parse SourceLink");
        }

        return value;
    }

    private static string GetName(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//h1[@itemprop='name']")
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ParseException("Failed to parse Name");
        }

        return text;
    }

    private static string GetType(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[@id='animeType']/div")
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ParseException("Failed to parse Type");
        }

        return text;
    }

    private static string GetStatus(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[contains(@class,'status-category')]//a")
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ParseException("Failed to parse Status");
        }

        return text;
    }

    private static int? GetTotalEpisodes(HtmlNode root)
    {
        var node = root.SelectSingleNode("//li[span[contains(text(),'Количество серий:')]]/div");
        node ??= root.SelectSingleNode("//li[span[text()='Вышло серий:']]//div");

        var text = node
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var xyMatch = _xyEpisodeCountRegex.Match(text);
        if (xyMatch.Success)
        {
            return int.TryParse(xyMatch.Groups[2].Value, out var yValue)
                ? yValue
                : null;
        }

        var xMatch = _xEpisodeCountRegex.Match(text);
        return xMatch.Success && int.TryParse(xMatch.Value, out var xValue)
            ? xValue
            : null;
    }

    private static int? GetReleasedEpisodes(HtmlNode root)
    {
        var node = root.SelectSingleNode("//li[span[text()='Вышло серий:']]//div");
        node ??= root.SelectSingleNode("//li[span[contains(text(),'Количество серий:')]]/div");

        var text = node
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var xyMatch = _xyEpisodeCountRegex.Match(text);
        if (xyMatch.Success)
        {
            return int.TryParse(xyMatch.Groups[1].Value, out var xValue)
                ? xValue
                : null;
        }

        var xMatch = _xEpisodeCountRegex.Match(text);
        return xMatch.Success && int.TryParse(xMatch.Value, out var singleValue)
            ? singleValue
            : null;
    }
}