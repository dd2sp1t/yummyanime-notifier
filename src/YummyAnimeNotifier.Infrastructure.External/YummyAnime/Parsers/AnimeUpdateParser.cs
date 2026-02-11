using YummyAnimeNotifier.Application.YummyAnime.Exceptions;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using HtmlAgilityPack;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime.Parsers;

internal class AnimeUpdateParser : IAnimeUpdateParser
{
    public ParsedAnimeUpdate[] Parse(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var root = doc.DocumentNode;

        var nodes = root.SelectNodes("//li[contains(@class,'one-video-update')]");
        if (nodes == null || nodes.Count == 0)
        {
            throw new ParseException("No updates found on the page");
        }

        var updates = nodes
            .Select(node =>
            {
                var animeName = GetAnimeName(node);
                var episodeNumber = GetEpisodeNumber(node);
                var translationRaw = GetTranslationRaw(node);

                return new ParsedAnimeUpdate(animeName, episodeNumber, translationRaw);
            })
            .ToArray();

        return updates;
    }

    private static string GetAnimeName(HtmlNode root)
    {
        var text = root
            .SelectSingleNode(".//span[contains(@class,'update-title')]")
            ?.InnerText
            ?.Trim();

        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ParseException("Failed to parse AnimeName");
        }

        return text;
    }

    private static int GetEpisodeNumber(HtmlNode root)
    {
        var boldNode = root.SelectSingleNode(".//span[contains(@class,'update-info')]/b");

        var raw = boldNode?.InnerText?.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new ParseException("Failed to parse EpisodeNumber (empty <b> node)");
        }

        // "123-я" → 123
        var digits = new string([.. raw.TakeWhile(char.IsDigit)]);
        if (int.TryParse(digits, out var value) == false)
        {
            throw new ParseException($"Failed to parse EpisodeNumber from '{raw}'");
        }

        return value;
    }

    private static string GetTranslationRaw(HtmlNode root)
    {
        var updateInfoNode = root.SelectSingleNode(".//span[contains(@class,'update-info')]");
        if (updateInfoNode == null)
        {
            throw new ParseException("Failed to locate update-info block");
        }

        var fullText = HtmlEntity.DeEntitize(updateInfoNode.InnerText).Trim();

        var markerIndex = fullText.IndexOf("серия:", StringComparison.OrdinalIgnoreCase);
        if (markerIndex < 0)
        {
            throw new ParseException($"Failed to locate 'серия:' marker in '{fullText}'");
        }

        var translationRaw = fullText[(markerIndex + "серия:".Length)..].Trim();
        if (string.IsNullOrWhiteSpace(translationRaw))
        {
            throw new ParseException("TranslationRaw is empty after 'серия:'");
        }

        return translationRaw;
    }
}