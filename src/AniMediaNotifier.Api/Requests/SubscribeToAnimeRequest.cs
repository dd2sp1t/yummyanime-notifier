namespace AniMediaNotifier.Api.Requests;

public record SubscribeToAnimeRequest(
    long TelegramUserId,
    string SourceLink
);