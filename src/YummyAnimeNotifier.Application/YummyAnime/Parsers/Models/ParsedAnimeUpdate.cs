namespace YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

public record ParsedAnimeUpdate(
    string AnimeName,
    int EpisodeNumber,
    string TranslationRaw);