namespace AniMediaNotifier.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AnimeId { get; private set; }
    public string Message { get; private set; }
    public bool IsSent { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }

    public static Notification Create(Guid userId, Guid animeId, string message)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = userId,
            AnimeId = animeId,
            Message = message,
            IsSent = false
        };
    }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTimeOffset.UtcNow;
    }
}