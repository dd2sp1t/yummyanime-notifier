using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbAnime
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int ExternalId { get; set; }
    public string SourceLink { get; set; }
    public string Name { get; set; }
    public AnimeType Type { get; set; }
    public AnimeStatus Status { get; set; }
    public int? ReleasedEpisodes { get; set; }
    public int? TotalEpisodes { get; set; }

    public ICollection<DbRelease> Releases { get; set; }
    public ICollection<DbAnimeTranslation> AnimeTranslations { get; set; }
    public ICollection<DbSubscription> Subscriptions { get; set; }
}