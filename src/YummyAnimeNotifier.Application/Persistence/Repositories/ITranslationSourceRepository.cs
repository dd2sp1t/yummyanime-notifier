using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface ITranslationSourceRepository
{
    Task<TranslationSource[]> FindAsync(
        TranslationType type,
        string[] names,
        CancellationToken cancellationToken = default);

    Task<TranslationSource[]> GetAsync(Guid[] ids, CancellationToken cancellationToken = default);

    void AddRange(TranslationSource[] translationSources);
}