using YummyAnimeNotifier.Domain.Events;

namespace YummyAnimeNotifier.Application.Events;

public record AnimeTranslationFinishedEvent(Guid AnimeId, Guid TranslationSourceId) : Event;