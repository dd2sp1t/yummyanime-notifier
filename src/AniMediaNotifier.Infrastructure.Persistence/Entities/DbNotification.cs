namespace AniMediaNotifier.Infrastructure.Persistence.Entities;

public class DbNotification
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid UserId { get; set; }
    public Guid AnimeId { get; set; }
    public int EpisodeNumber { get; set; }
    public string Message { get; set; }
    public bool IsSent { get; set; }
    public DateTimeOffset? SentAt { get; set; }

    public DbUser User { get; set; }
    public DbAnime Anime { get; set; }
}