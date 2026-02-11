using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities;

public class DbTranslationSource
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public TranslationType Type { get; set; }
    public string Name { get; set; }

    public ICollection<DbRelease> Releases { get; set; }
    public ICollection<DbAnimeTranslation> AnimeTranslations { get; set; }
    public ICollection<DbSubscription> Subscriptions { get; set; }
}