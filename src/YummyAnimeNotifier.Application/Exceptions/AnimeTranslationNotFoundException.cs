using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Exceptions;

public class AnimeTranslationNotFoundException : ApplicationException
{
    public Guid AnimeId { get; }
    public TranslationType TranslationType { get; }
    public string TranslationSourceName { get; }

    public AnimeTranslationNotFoundException(
        Guid animeId,
        TranslationType translationType,
        string translationSourceName)
        : base("The anime translation was not found")
    {
        AnimeId = animeId;
        TranslationType = translationType;
        TranslationSourceName = translationSourceName;
    }
}