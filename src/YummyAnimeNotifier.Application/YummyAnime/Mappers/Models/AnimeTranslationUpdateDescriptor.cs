using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;

public record AnimeTranslationUpdateDescriptor(
    string AnimeName,
    TranslationType TranslationType,
    string TranslationSourceName,
    int EpisodeNumber);