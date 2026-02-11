using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Domain.Entities;

public class AnimeTranslation
{
    public Guid AnimeId { get; private set; }
    public Guid TranslationSourceId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public AnimeStatus Status { get; private set; }
    public int? TotalEpisodes { get; private set; }
    public int? ReleasedEpisodes { get; private set; }

    public bool TryUpdateReleasedEpisodes(int episodeNumber)
    {
        if (episodeNumber <= ReleasedEpisodes)
        {
            return false;
        }

        ReleasedEpisodes = episodeNumber;
        UpdatedAt = DateTimeOffset.UtcNow;

        if (TotalEpisodes.HasValue && ReleasedEpisodes >= TotalEpisodes)
        {
            Status = AnimeStatus.Finished;
        }

        return true;
    }

    public bool IsFinished => Status == AnimeStatus.Finished;

    public static AnimeTranslation Create(
        Guid animeId,
        Guid translationSourceId,
        AnimeStatus status,
        int? totalEpisodes,
        int? releasedEpisodes)
    {
        return new AnimeTranslation
        {
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = status,
            TotalEpisodes = totalEpisodes,
            ReleasedEpisodes = releasedEpisodes
        };
    }

    public static AnimeTranslation FromExisting(
        Guid animeId,
        Guid translationSourceId,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        AnimeStatus status,
        int? totalEpisodes,
        int? releasedEpisodes)
    {
        return new AnimeTranslation
        {
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            Status = status,
            TotalEpisodes = totalEpisodes,
            ReleasedEpisodes = releasedEpisodes
        };
    }
}