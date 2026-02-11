using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Application.Events;

public record NewEpisodeDetectedEvent(
    Guid AnimeId,
    Guid TranslationSourceId,
    int EpisodeNumber
) : Event;