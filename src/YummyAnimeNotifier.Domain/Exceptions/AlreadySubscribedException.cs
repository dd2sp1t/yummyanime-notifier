namespace YummyAnimeNotifier.Domain.Exceptions;

public class AlreadySubscribedException : DomainException
{
    public Guid UserId { get; }
    public Guid AnimeId { get; }
    public Guid TranslationSourceId { get; }

    public AlreadySubscribedException(Guid userId, Guid animeId, Guid translationSourceId)
        : base("User is already subscribed to the anime translation")
    {
        UserId = userId;
        AnimeId = animeId;
        TranslationSourceId = translationSourceId;
    }
}