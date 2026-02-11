namespace YummyAnimeNotifier.Domain.Exceptions;

public class AlreadyUnsubscribedException : DomainException
{
    public Guid UserId { get; }
    public Guid AnimeId { get; }

    public AlreadyUnsubscribedException(Guid userId, Guid animeId)
        : base("User is already unsubscribed from the anime")
    {
        UserId = userId;
        AnimeId = animeId;
    }
}