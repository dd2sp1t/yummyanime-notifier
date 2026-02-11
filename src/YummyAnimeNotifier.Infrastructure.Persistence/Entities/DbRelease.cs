namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbRelease
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid AnimeId { get; set; }
    public Guid TranslationSourceId { get; set; }
    public int EpisodeNumber { get; set; }

    public DbAnime Anime { get; set; }
    public DbTranslationSource TranslationSource { get; set; }
    public ICollection<DbNotification> Notifications { get; set; }
}