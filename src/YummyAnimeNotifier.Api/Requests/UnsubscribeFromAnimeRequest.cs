namespace YummyAnimeNotifier.Api.Requests;

public record UnsubscribeFromAnimeRequest(
    long TelegramUserId,
    string RuName
);