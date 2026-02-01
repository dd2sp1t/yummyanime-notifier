namespace AniMediaNotifier.Domain.Exceptions;

public class AlreadySubscribedException : DomainException
{
    public Guid UserId { get; }
    public Guid AnimeId { get; }

    public AlreadySubscribedException(Guid userId, Guid animeId)
        : base("User is already subscribed to the anime")
    {
        UserId = userId;
        AnimeId = animeId;
    }
}