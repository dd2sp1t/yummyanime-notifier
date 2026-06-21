namespace YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

public record ParsedAnimeTranslationUpdate(
    string AnimeName,
    string TranslationRaw,
    int EpisodeNumber);