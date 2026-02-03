using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Infrastructure.Persistence.Entities;

public class DbNotification
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid AnimeId { get; set; }
    public string RuName { get; set; }
    public string Url { get; set; }
    public int? TotalEpisodes { get; set; }
    public int EpisodeNumber { get; set; }
    public NotificationStatus Status { get; set; }
    public string Error { get; set; }

    public DbUser User { get; set; }
    public DbAnime Anime { get; set; }
}