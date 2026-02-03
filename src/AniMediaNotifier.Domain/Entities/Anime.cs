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
    public AnimeType Type { get; private set; }
    public AnimeStatus Status { get; private set; }
    public int ReleasedEpisodes { get; private set; }
    public int? TotalEpisodes { get; private set; }

    public bool TryUpdateReleasedEpisodes(int episodeNumber)
    {
        if (episodeNumber <= ReleasedEpisodes)
        {
            return false;
        }

        ReleasedEpisodes = episodeNumber;

        if (TotalEpisodes.HasValue && ReleasedEpisodes >= TotalEpisodes)
        {
            Status = AnimeStatus.Finished;
        }

        return true;
    }

    public bool IsFinished => Status == AnimeStatus.Finished;

    #region Create methods

    private const int MaxNameLength = 255;
    private const int MinYear = 1900;

    public static Anime Create(
        string sourceLink,
        string originalName,
        string ruName,
        int year,
        AnimeType type,
        AnimeStatus status,
        int releasedEpisodes,
        int? totalEpisodes)
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
            SourceLink = sourceLink,
            OriginalName = originalName,
            RuName = ruName,
            Year = year,
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
        string sourceLink,
        string originalName,
        string ruName,
        int year,
        AnimeType type,
        AnimeStatus status,
        int releasedEpisodes,
        int? totalEpisodes)
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
            ReleasedEpisodes = releasedEpisodes,
            TotalEpisodes = totalEpisodes
        };
    }

    #endregion
}