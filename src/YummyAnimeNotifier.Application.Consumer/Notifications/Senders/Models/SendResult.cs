namespace YummyAnimeNotifier.Application.Consumer.Notifications.Senders.Models;

public record SendResult(
    bool Success,
    string Error
);