namespace AniMediaNotifier.Application.Persistence.Repositories;

public interface IAnimeRepository
{
    Task<Domain.Entities.Anime> FindBySourceLinkAsync(string sourceLink, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Anime> FindByRuNameAsync(string ruName, CancellationToken cancellationToken = default);
    Task<Domain.Entities.Anime> GetAsync(Guid id, CancellationToken cancellationToken = default);
    void Add(Domain.Entities.Anime anime);
    Task UpdateAsync(Domain.Entities.Anime anime, CancellationToken cancellationToken = default);
}