using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Application.Events;

public record ReleaseCreatedEvent(Guid ReleaseId) : Event;