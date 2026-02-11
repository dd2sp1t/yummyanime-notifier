namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbUser
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public long TelegramUserId { get; set; }

    public ICollection<DbSubscription> Subscriptions { get; set; }
    public ICollection<DbNotification> Notifications { get; set; }
}