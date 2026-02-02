using AniMediaNotifier.Domain.Events;

namespace AniMediaNotifier.Application.Events;

public record AnimeFinishedEvent(Guid AnimeId) : Event;