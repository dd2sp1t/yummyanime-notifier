namespace YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

public record ParsedAnime(
    int ExternalId,
    string SourceLink,
    string Name,
    string TypeRaw,
    string StatusRaw,
    int? TotalEpisodes,
    int? ReleasedEpisodes);