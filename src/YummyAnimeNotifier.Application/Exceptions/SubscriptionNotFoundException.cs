namespace YummyAnimeNotifier.Application.Exceptions;

public class SubscriptionNotFoundException : ApplicationException
{
    public Guid UserId { get; }
    public Guid AnimeId { get; }

    public SubscriptionNotFoundException(Guid userId, Guid animeId)
        : base("The subscription was not found")
    {
        UserId = userId;
        AnimeId = animeId;
    }
}