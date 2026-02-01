namespace AniMediaNotifier.Application.Events;

public record AnimeFinishedEvent(Guid AnimeId) : Event;