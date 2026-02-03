using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Persistence.Repositories;

public interface IOutboxMessageRepository
{
    void Add(OutboxMessage outboxMessage);
    void AddRange(OutboxMessage[] outboxMessages);
    Task UpdateRangeAsync(OutboxMessage[] outboxMessages, CancellationToken cancellationToken = default);
    Task<OutboxMessage[]> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default);
}