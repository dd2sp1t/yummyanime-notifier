namespace AniMediaNotifier.Application.Notifications.Senders.Models;

public record SendResult(
    bool Success,
    string Error
);