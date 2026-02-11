using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;

public record AnimeTranslationDescriptor(
    TranslationType TranslationType,
    string TranslationSourceName,
    int MaxEpisodeNumber);