using YummyAnimeNotifier.Domain.Entities;

namespace YummyAnimeNotifier.Application.Persistence.Repositories;

public interface IOutboxMessageRepository
{
    void Add(OutboxMessage outboxMessage);
    void AddRange(OutboxMessage[] outboxMessages);
    Task UpdateRangeAsync(OutboxMessage[] outboxMessages, CancellationToken cancellationToken = default);
    Task<OutboxMessage[]> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default);
}