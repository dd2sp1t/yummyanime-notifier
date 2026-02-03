namespace AniMediaNotifier.Application.Notifications.Senders;

public record SendResult(
    bool Success,
    string Error
);