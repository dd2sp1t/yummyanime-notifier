using AniMediaNotifier.Domain.Enums;
using AniMediaNotifier.Domain.Exceptions;

namespace AniMediaNotifier.Domain.Entities;

public class Anime
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string SourceLink { get; private set; }
    public string OriginalName { get; private set; }
    public string RuName { get; private set; }
    public int Year { get; private set; }
    public AnimeTypeEnum Type { get; private set; }
    public AnimeStatusEnum Status { get; private set; }
    public int ReleasedEpisodeCount { get; private set; }
    public int? TotalEpisodeCount { get; private set; }

    public bool TryUpdateReleasedEpisode(int episodeNumber)
    {
        if (episodeNumber <= ReleasedEpisodeCount)
        {
            return false;
        }

        ReleasedEpisodeCount = episodeNumber;

        if (TotalEpisodeCount.HasValue && ReleasedEpisodeCount >= TotalEpisodeCount)
        {
            Status = AnimeStatusEnum.Finished;
        }

        return true;
    }

    public bool IsFinished => Status == AnimeStatusEnum.Finished;

    #region Create methods

    private const int MaxNameLength = 255;
    private const int MinYear = 1900;

    public static Anime Create(
        string sourceLink,
        string originalName,
        string ruName,
        int year,
        AnimeTypeEnum type,
        AnimeStatusEnum status,
        int releasedEpisodeCount,
        int? totalEpisodeCount)
    {
        if (string.IsNullOrWhiteSpace(sourceLink))
        {
            throw new InvalidAnimeStateException("SourceLink is empty");
        }

        if (string.IsNullOrWhiteSpace(originalName))
        {
            throw new InvalidAnimeStateException("OriginalName is empty");
        }

        if (originalName.Length > MaxNameLength)
        {
            throw new InvalidAnimeStateException("OriginalName is too long");
        }

        if (string.IsNullOrWhiteSpace(ruName))
        {
            throw new InvalidAnimeStateException("RuName is empty");
        }

        if (ruName.Length > MaxNameLength)
        {
            throw new InvalidAnimeStateException("RuName is too long");
        }

        if (year < MinYear || year > DateTime.UtcNow.Year + 1)
        {
            throw new InvalidAnimeStateException("Year is invalid");
        }

        if (type == AnimeTypeEnum.None)
        {
            throw new InvalidAnimeStateException("Anime type is not specified");
        }

        if (status == AnimeStatusEnum.None)
        {
            throw new InvalidAnimeStateException("Anime status is not specified");
        }

        var anime = new Anime
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            SourceLink = sourceLink,
            OriginalName = originalName,
            RuName = ruName,
            Year = year,
            Type = type,
            Status = status,
            ReleasedEpisodeCount = releasedEpisodeCount,
            TotalEpisodeCount = totalEpisodeCount
        };

        return anime;
    }

    public static Anime FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        string sourceLink,
        string originalName,
        string ruName,
        int year,
        AnimeTypeEnum type,
        AnimeStatusEnum status,
        int releasedEpisodeCount,
        int? totalEpisodeCount)
    {
        return new Anime
        {
            Id = id,
            CreatedAt = createdAt,
            SourceLink = sourceLink,
            OriginalName = originalName,
            RuName = ruName,
            Year = year,
            Type = type,
            Status = status,
            ReleasedEpisodeCount = releasedEpisodeCount,
            TotalEpisodeCount = totalEpisodeCount
        };
    }

    #endregion
}