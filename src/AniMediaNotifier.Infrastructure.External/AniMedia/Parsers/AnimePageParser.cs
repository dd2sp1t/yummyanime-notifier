using AniMediaNotifier.Application.AniMedia.Exceptions;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.AniMedia.Parsers.Models;
using HtmlAgilityPack;

namespace AniMediaNotifier.Infrastructure.External.AniMedia.Parsers;

internal class AnimePageParser : IAnimePageParser
{
    public ParsedAnimeInfo Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var root = doc.DocumentNode;

        return new ParsedAnimeInfo(
            GetSourceLink(root),
            GetOriginalName(root),
            GetRuName(root),
            GetYear(root),
            GetType(root),
            GetStatus(root),
            GetReleasedEpisodeCount(root),
            GetTotalEpisodeCount(root));
    }

    private static string GetSourceLink(HtmlNode root)
    {
        var value = root
            .SelectSingleNode("//meta[@property='og:url']")
            ?.GetAttributeValue("content", null)
            ?.Trim();

        if (Uri.TryCreate(value, UriKind.Absolute, out var uri) == false)
        {
            throw new AnimePageParseException("Failed to parse SourceLink");
        }

        return uri.AbsolutePath;
    }

    private static string GetOriginalName(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//div[contains(@class,'pmovie__main-info')]")
            ?.InnerText
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new AnimePageParseException("Failed to parse OriginalName");
        }

        return text;
    }

    private static string GetRuName(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//header[contains(@class,'pmovie__header')]//h1")
            ?.InnerText
            .Trim('«', '»', ' ', '\n', '\t');

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new AnimePageParseException("Failed to parse RuName");
        }

        return text;
    }

    private static int GetYear(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[span[text()='Год:']]/a")
            ?.InnerText
            .Trim();

        if (int.TryParse(text, out var value) == false)
        {
            throw new AnimePageParseException("Failed to parse Year");
        }

        return value;
    }

    private static string GetType(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[span[text()='Тип:']]/a")
            ?.InnerText
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new AnimePageParseException("Failed to parse Type");
        }

        return text;
    }

    private static string GetStatus(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[span[text()='Статус:']]/a")
            ?.InnerText
            .Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new AnimePageParseException("Failed to parse Status");
        }

        return text;
    }

    private static int GetReleasedEpisodeCount(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[.//span[text()='Вышла серия:']]//div[@class='spanser']/span")
            ?.InnerText
            .Trim();

        // /2189-shkola-vverh-dnom-2026.html
        if (text == "-")
        {
            return 0;
        }

        if (int.TryParse(text, out var value) == false)
        {
            throw new AnimePageParseException("Failed to parse ReleasedEpisodeCount");
        }

        return value;
    }

    private static int? GetTotalEpisodeCount(HtmlNode root)
    {
        var text = root
            .SelectSingleNode("//li[.//span[text()='Вышла серия:']]//div[@class='spanser']/text()[normalize-space()]")
            ?.InnerText
            .Trim('+', ' ', '\n', '\t');

        return int.TryParse(text, out var value)
            ? value
            : null;
    }
}