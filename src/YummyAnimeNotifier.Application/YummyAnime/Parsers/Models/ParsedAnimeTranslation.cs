namespace YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

public record ParsedAnimeTranslation(
    string TranslationRaw,
    int MaxEpisodeNumber
);