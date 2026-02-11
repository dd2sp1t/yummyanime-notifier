using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Domain.Entities;

public class TranslationSource
{
    public Guid Id { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public TranslationType Type { get; private set; }
    public string Name { get; private set; }

    public static TranslationSource Create(TranslationType type, string name)
    {
        return new TranslationSource
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTimeOffset.UtcNow,
            Type = type,
            Name = name
        };
    }

    public static TranslationSource FromExistings(Guid id, DateTimeOffset createdAt, TranslationType type, string name)
    {
        return new TranslationSource
        {
            Id = id,
            CreatedAt = createdAt,
            Type = type,
            Name = name
        };
    }
}