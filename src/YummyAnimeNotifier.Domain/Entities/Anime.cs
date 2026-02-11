using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Domain.Exceptions;

namespace YummyAnimeNotifier.Domain.Entities;

public class Anime
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public int ExternalId { get; private set; }
    public string SourceLink { get; private set; }
    public string Name { get; private set; }
    public AnimeType Type { get; private set; }
    public AnimeStatus Status { get; private set; }
    public int? ReleasedEpisodes { get; private set; }
    public int? TotalEpisodes { get; private set; }

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

    #region Create methods

    private const int MaxNameLength = 255;

    public static Anime Create(
        int externalId,
        string sourceLink,
        string ruName,
        AnimeType type,
        AnimeStatus status,
        int? releasedEpisodes,
        int? totalEpisodes)
    {
        if (externalId == default)
        {
            throw new InvalidAnimeStateException("ExternalId is not specified");
        }

        if (string.IsNullOrWhiteSpace(sourceLink))
        {
            throw new InvalidAnimeStateException("SourceLink is empty");
        }

        if (string.IsNullOrWhiteSpace(ruName))
        {
            throw new InvalidAnimeStateException("RuName is empty");
        }

        if (ruName.Length > MaxNameLength)
        {
            throw new InvalidAnimeStateException("RuName is too long");
        }

        if (type == AnimeType.None)
        {
            throw new InvalidAnimeStateException("Anime type is not specified");
        }

        if (status == AnimeStatus.None)
        {
            throw new InvalidAnimeStateException("Anime status is not specified");
        }

        var anime = new Anime
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            ExternalId = externalId,
            SourceLink = sourceLink,
            Name = ruName,
            Type = type,
            Status = status,
            ReleasedEpisodes = releasedEpisodes,
            TotalEpisodes = totalEpisodes
        };

        return anime;
    }

    public static Anime FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        int externalId,
        string sourceLink,
        string name,
        AnimeType type,
        AnimeStatus status,
        int? releasedEpisodes,
        int? totalEpisodes)
    {
        return new Anime
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            ExternalId = externalId,
            SourceLink = sourceLink,
            Name = name,
            Type = type,
            Status = status,
            ReleasedEpisodes = releasedEpisodes,
            TotalEpisodes = totalEpisodes
        };
    }

    #endregion
}