namespace YummyAnimeNotifier.Domain.Entities;

public class Release
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid AnimeId { get; private set; }
    public Guid TranslationSourceId { get; private set; }
    public int EpisodeNumber { get; private set; }

    public static Release Create(Guid animeId, Guid translationSourceId, int episodeNumber)
    {
        return new Release
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            EpisodeNumber = episodeNumber
        };
    }

    public static Release FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        Guid animeId,
        Guid translationSourceId,
        int episodeNumber)
    {
        return new Release
        {
            Id = id,
            CreatedAt = createdAt,
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            EpisodeNumber = episodeNumber
        };
    }
}