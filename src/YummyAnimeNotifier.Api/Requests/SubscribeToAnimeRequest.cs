using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Api.Requests;

public record SubscribeToAnimeRequest(
    long TelegramUserId,
    string SourceLink,
    TranslationType TranslationType,
    string TranslationSourceName
);