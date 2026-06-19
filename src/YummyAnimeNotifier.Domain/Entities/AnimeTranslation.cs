using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Domain.Entities;

public class AnimeTranslation
{
    public Guid AnimeId { get; private set; }
    public Guid TranslationSourceId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public AnimeTranslationStatus Status { get; private set; }
    public int? TotalEpisodes { get; private set; }
    public int ReleasedEpisodes { get; private set; }

    public bool TryUpdateReleasedEpisodes(int episodeNumber)
    {
        if (episodeNumber <= ReleasedEpisodes)
        {
            return false;
        }

        ReleasedEpisodes = episodeNumber;
        UpdatedAt = DateTimeOffset.UtcNow;

        if (TotalEpisodes.HasValue && ReleasedEpisodes == TotalEpisodes)
        {
            Status = AnimeTranslationStatus.Finished;
        }

        return true;
    }

    public bool IsFinished => Status == AnimeTranslationStatus.Finished;

    public static AnimeTranslation Create(
        Guid animeId,
        Guid translationSourceId,
        int? totalEpisodes,
        int releasedEpisodes)
    {
        return new AnimeTranslation
        {
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            CreatedAt = DateTimeOffset.UtcNow,
            Status = totalEpisodes.HasValue && totalEpisodes == releasedEpisodes
                ? AnimeTranslationStatus.Finished
                : AnimeTranslationStatus.Ongoing,
            TotalEpisodes = totalEpisodes,
            ReleasedEpisodes = releasedEpisodes
        };
    }

    public static AnimeTranslation FromExisting(
        Guid animeId,
        Guid translationSourceId,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        AnimeTranslationStatus status,
        int? totalEpisodes,
        int releasedEpisodes)
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