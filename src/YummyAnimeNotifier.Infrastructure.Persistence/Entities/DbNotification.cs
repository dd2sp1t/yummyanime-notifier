using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbNotification
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid ReleaseId { get; set; }
    public Guid UserId { get; set; }
    public string AnimeName { get; set; }
    public string Url { get; set; }
    public int? TotalEpisodes { get; set; }
    public int EpisodeNumber { get; set; }
    public string TranslationSourceName { get; set; }
    public NotificationStatus Status { get; set; }
    public string Error { get; set; }

    public DbRelease Release { get; set; }
    public DbUser User { get; set; }
}