using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Domain.Exceptions;

namespace YummyAnimeNotifier.Domain.Entities;

public class Subscription
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? UpdatedAt { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AnimeId { get; private set; }
    public Guid TranslationSourceId { get; private set; }
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

    public void Restore(AnimeTranslationStatus animeTranslationStatus)
    {
        if (animeTranslationStatus == AnimeTranslationStatus.Finished)
        {
            throw new AnimeTranslationAlreadyFinishedException(AnimeId, TranslationSourceId);
        }

        if (IsDeleted == false)
        {
            throw new AlreadySubscribedException(UserId, AnimeId, TranslationSourceId);
        }

        IsDeleted = false;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public static Subscription Create(
        Guid userId,
        Guid animeId,
        Guid translationSourceId,
        AnimeTranslationStatus animeTranslationStatus)
    {
        if (animeTranslationStatus == AnimeTranslationStatus.Finished)
        {
            throw new AnimeTranslationAlreadyFinishedException(animeId, translationSourceId);
        }

        return new Subscription
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            UserId = userId,
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            IsDeleted = false
        };
    }

    public static Subscription FromExisting(
        Guid id,
        DateTimeOffset createdAt,
        DateTimeOffset? updatedAt,
        Guid userId,
        Guid animeId,
        Guid translationSourceId,
        bool isDeleted)
    {
        return new Subscription
        {
            Id = id,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt,
            UserId = userId,
            AnimeId = animeId,
            TranslationSourceId = translationSourceId,
            IsDeleted = isDeleted
        };
    }
}