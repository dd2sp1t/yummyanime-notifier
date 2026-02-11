using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Application.Events;

public record AnimeFinishedEvent(Guid AnimeId) : Event;