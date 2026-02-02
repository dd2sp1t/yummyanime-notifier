namespace AniMediaNotifier.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AnimeId { get; private set; }
    public int EpisodeNumber { get; private set; }
    public string Message { get; private set; }
    public bool IsSent { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }

    public void MarkAsSent()
    {
        IsSent = true;
        SentAt = DateTimeOffset.UtcNow;
    }

    public static Notification Create(Guid userId, Guid animeId, int episodeNumber, string message)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = userId,
            AnimeId = animeId,
            EpisodeNumber = episodeNumber,
            Message = message,
            IsSent = false
        };
    }

    public static Notification FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        Guid userId,
        Guid animeId,
        int episodeNumber,
        string message,
        bool isSent,
        DateTimeOffset? sentAt)
    {
        return new Notification
        {
            Id = id,
            CreatedAt = createdAt,
            UserId = userId,
            AnimeId = animeId,
            EpisodeNumber = episodeNumber,
            Message = message,
            IsSent = isSent,
            SentAt = sentAt
        };
    }
}