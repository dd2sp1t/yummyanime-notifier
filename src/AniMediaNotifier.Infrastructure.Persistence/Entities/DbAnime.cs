using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Infrastructure.Persistence.Entities;

public class DbAnime
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string SourceLink { get; set; }
    public string OriginalName { get; set; }
    public string RuName { get; set; }
    public int Year { get; set; }
    public AnimeTypeEnum Type { get; set; }
    public AnimeStatusEnum Status { get; set; }
    public int ReleasedEpisodeCount { get; set; }
    public int? TotalEpisodeCount { get; set; }

    public ICollection<DbSubscription> Subscriptions { get; set; }
    public ICollection<DbNotification> Notifications { get; set; }
}