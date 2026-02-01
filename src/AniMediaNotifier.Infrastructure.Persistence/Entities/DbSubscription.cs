namespace AniMediaNotifier.Infrastructure.Persistence.Entities;

public class DbSubscription
{
    public Guid UserId { get; set; }
    public Guid AnimeId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public DbUser User { get; set; }
    public DbAnime Anime { get; set; }
}