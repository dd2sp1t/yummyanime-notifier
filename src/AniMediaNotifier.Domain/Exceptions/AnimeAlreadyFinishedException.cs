namespace AniMediaNotifier.Domain.Exceptions;

public class AnimeAlreadyFinishedException : DomainException
{
    public Guid AnimeId { get; }

    public AnimeAlreadyFinishedException(Guid animeId)
        : base("The anime has already finished")
    {
        AnimeId = animeId;
    }
}