using AniMediaNotifier.Domain.Events;

namespace AniMediaNotifier.Application.Events;

public record NewEpisodeDetectedEvent(
    Guid AnimeId,
    int EpisodeNumber
) : Event;