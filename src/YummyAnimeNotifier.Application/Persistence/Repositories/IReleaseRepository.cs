using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface IReleaseRepository
{
    Task<Release> GetAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Release release);
}