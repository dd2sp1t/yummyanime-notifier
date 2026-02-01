using System.Text.Json;
using AniMediaNotifier.Application.Notifications;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Infrastructure.External.NotificationSenders;

public class MockNotificationSender : INotificationSender
{
    public Task SendAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Notification {JsonSerializer.Serialize(notification)} was sent");

        return Task.CompletedTask;
    }
}