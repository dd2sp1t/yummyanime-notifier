using System.Text.Json;
using YummyAnimeNotifier.Application.Notifications.Senders;
using YummyAnimeNotifier.Application.Notifications.Senders.Models;
using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Infrastructure.External.Mock;

public class MockNotificationSender : INotificationSender
{
    public Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Notification {JsonSerializer.Serialize(notification)} was sent");

        return Task.FromResult(new SendResult(true, null));
    }
}