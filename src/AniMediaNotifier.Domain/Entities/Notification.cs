using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AnimeId { get; private set; }
    public string RuName { get; private set; }
    public string Url { get; private set; }
    public int? TotalEpisodes { get; private set; }
    public int EpisodeNumber { get; private set; }
    public NotificationStatus Status { get; private set; }
    public string Error { get; private set; }

    public void MarkAsSent()
    {
        Status = NotificationStatus.Sent;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Status = NotificationStatus.Failed;
        Error = error;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Notification Create(
        Guid userId,
        Guid animeId,
        string ruName,
        string url,
        int? totalEpisodes,
        int episodeNumber)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = userId,
            AnimeId = animeId,
            RuName = ruName,
            Url = url,
            TotalEpisodes = totalEpisodes,
            EpisodeNumber = episodeNumber,
            Status = NotificationStatus.Pending
        };
    }

    public static Notification FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        Guid userId,
        Guid animeId,
        string ruName,
        string url,
        int? totalEpisodes,
        int episodeNumber,
        NotificationStatus status,
        string error)
    {
        return new Notification
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            UserId = userId,
            AnimeId = animeId,
            RuName = ruName,
            Url = url,
            TotalEpisodes = totalEpisodes,
            EpisodeNumber = episodeNumber,
            Status = status,
            Error = error
        };
    }
}