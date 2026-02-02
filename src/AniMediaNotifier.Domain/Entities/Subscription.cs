using AniMediaNotifier.Domain.Enums;
using AniMediaNotifier.Domain.Exceptions;

namespace AniMediaNotifier.Domain.Entities;

public class Subscription
{
    public Guid UserId { get; private set; }
    public Guid AnimeId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Cancel()
    {
        if (IsDeleted)
        {
            throw new AlreadyUnsubscribedException(UserId, AnimeId);
        }

        IsDeleted = true;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Restore(AnimeStatus animeStatus)
    {
        if (animeStatus == AnimeStatus.Finished)
        {
            throw new AnimeAlreadyFinishedException(AnimeId);
        }

        if (IsDeleted == false)
        {
            throw new AlreadySubscribedException(UserId, AnimeId);
        }

        IsDeleted = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Subscription Create(Guid userId, Guid animeId, AnimeStatus animeStatus)
    {
        if (animeStatus == AnimeStatus.Finished)
        {
            throw new AnimeAlreadyFinishedException(animeId);
        }

        return new Subscription
        {
            UserId = userId,
            AnimeId = animeId,
            CreatedAt = DateTimeOffset.UtcNow,
            IsDeleted = false
        };
    }

    public static Subscription FromExisting(
        Guid userId,
        Guid animeId,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        bool isDeleted)
    {
        return new Subscription
        {
            UserId = userId,
            AnimeId = animeId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            IsDeleted = isDeleted
        };
    }
}