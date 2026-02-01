namespace AniMediaNotifier.Application.AniMedia.Parsers.Models;

public record ParsedAnimeInfo(
    string SourceLink,
    string OriginalName,
    string RuName,
    int Year,
    string TypeRaw,
    string StatusRaw,
    int ReleasedEpisodeCount,
    int? TotalEpisodeCount);