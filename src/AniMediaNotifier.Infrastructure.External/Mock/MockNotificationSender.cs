using System.Text.Json;
using AniMediaNotifier.Application.Notifications.Senders;
using AniMediaNotifier.Application.Notifications.Senders.Models;
using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Infrastructure.External.Mock;

public class MockNotificationSender : INotificationSender
{
    public Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Notification {JsonSerializer.Serialize(notification)} was sent");

        return Task.FromResult(new SendResult(true, null));
    }
}