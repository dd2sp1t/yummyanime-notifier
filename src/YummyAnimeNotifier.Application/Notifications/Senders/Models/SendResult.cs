namespace YummyAnimeNotifier.Application.Notifications.Senders.Models;

public record SendResult(
    bool Success,
    string Error
);