using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbAnimeTranslation
{
    public Guid AnimeId { get; set; }
    public Guid TranslationSourceId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public AnimeTranslationStatus Status { get; set; }
    public int? TotalEpisodes { get; set; }
    public int ReleasedEpisodes { get; set; }

    public DbAnime Anime { get; set; }
    public DbTranslationSource TranslationSource { get; set; }
}