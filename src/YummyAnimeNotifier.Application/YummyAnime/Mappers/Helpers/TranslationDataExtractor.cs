using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers.Helpers;

internal static class TranslationDataExtractor
{
    private const string SubtitlesPrefix = "Субтитры";
    private const string VoicePrefix = "Озвучка";
    private const string NoName = "__default__";

    public static (TranslationType Type, string Name) Extract(string raw)
    {
        var type = ExtractType(raw);
        var name = ExtractName(raw);

        return (type, name);
    }

    private static TranslationType ExtractType(string raw)
    {
        if (raw.StartsWith(SubtitlesPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return TranslationType.Subtitles;
        }
        if (raw.StartsWith(VoicePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return TranslationType.Voice;
        }

        throw new InvalidOperationException($"Unknown translation type: '{raw}'");
    }

    private static string ExtractName(string raw)
    {
        if (raw.StartsWith(SubtitlesPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return ExtractName(raw, SubtitlesPrefix);
        }
        if (raw.StartsWith(VoicePrefix, StringComparison.OrdinalIgnoreCase))
        {
            return ExtractName(raw, VoicePrefix);
        }

        throw new InvalidOperationException($"Cannot extract translation name from '{raw}'");
    }

    private static string ExtractName(string raw, string prefix)
    {
        var name = raw[prefix.Length..].Trim();

        return string.IsNullOrWhiteSpace(name)
            ? NoName
            : name;
    }
}