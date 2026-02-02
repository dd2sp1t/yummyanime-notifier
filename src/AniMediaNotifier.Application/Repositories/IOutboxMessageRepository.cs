using AniMediaNotifier.Domain.Entities;

namespace AniMediaNotifier.Application.Repositories;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken = default);
    Task AddRangeAsync(OutboxMessage[] outboxMessages, CancellationToken cancellationToken = default);
    Task UpdateRangeAsync(OutboxMessage[] outboxMessages, CancellationToken cancellationToken = default);
    Task<OutboxMessage[]> GetPendingAsync(int maxCount, CancellationToken cancellationToken = default);
}