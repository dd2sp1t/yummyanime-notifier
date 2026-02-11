using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;

public record AnimeUpdateDescriptor(
    string AnimeName,
    int EpisodeNumber,
    TranslationType TranslationType,
    string TranslationSourceName);