using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid ReleaseId { get; private set; }
    public Guid UserId { get; private set; }
    public string AnimeName { get; private set; }
    public string Url { get; private set; }
    public int? TotalEpisodes { get; private set; }
    public int EpisodeNumber { get; private set; }
    public string TranslationSourceName { get; private set; }
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
        Guid releaseId,
        string animeName,
        string url,
        int? totalEpisodes,
        int episodeNumber,
        string translationSourceName)
    {
        return new Notification
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = userId,
            ReleaseId = releaseId,
            AnimeName = animeName,
            Url = url,
            TotalEpisodes = totalEpisodes,
            EpisodeNumber = episodeNumber,
            TranslationSourceName = translationSourceName,
            Status = NotificationStatus.Pending
        };
    }

    public static Notification FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        Guid userId,
        Guid releaseId,
        string animeName,
        string url,
        int? totalEpisodes,
        int episodeNumber,
        string translationSourceName,
        NotificationStatus status,
        string error)
    {
        return new Notification
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            UserId = userId,
            ReleaseId = releaseId,
            AnimeName = animeName,
            Url = url,
            TotalEpisodes = totalEpisodes,
            EpisodeNumber = episodeNumber,
            TranslationSourceName = translationSourceName,
            Status = status,
            Error = error
        };
    }
}