namespace YummyAnimeNotifier.Domain.Exceptions;

public class AnimeTranslationAlreadyFinishedException : DomainException
{
    public Guid AnimeId { get; }
    public Guid TranslationSourceId { get; }

    public AnimeTranslationAlreadyFinishedException(Guid animeId, Guid translationSourceId)
        : base("The anime translation has already finished")
    {
        AnimeId = animeId;
        TranslationSourceId = translationSourceId;
    }
}