using AniMediaNotifier.Application.Persistence.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Domain.Enums;
using AniMediaNotifier.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace AniMediaNotifier.Infrastructure.Persistence.Repositories;

internal class OutboxMessageRepository : IOutboxMessageRepository
{
    private readonly AniMediaDbContext _dbContext;

    public OutboxMessageRepository(AniMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(OutboxMessage message)
    {
        var dbMessage = new DbOutboxMessage
        {
            Id = message.Id,
            CreatedAt = message.CreatedAt,
            EventType = message.EventType,
            Payload = message.Payload,
            Status = message.Status,
            Error = message.Error
        };
        _dbContext.OutboxMessages.Add(dbMessage);
    }

    public void AddRange(OutboxMessage[] messages)
    {
        var dbMessages = messages
            .Select(m => new DbOutboxMessage
            {
                Id = m.Id,
                CreatedAt = m.CreatedAt,
                EventType = m.EventType,
                Payload = m.Payload,
                Status = m.Status,
                Error = m.Error
            })
            .ToArray();
        _dbContext.OutboxMessages.AddRange(dbMessages);
    }

    public async Task UpdateRangeAsync(OutboxMessage[] messages, CancellationToken cancellationToken)
    {
        if (messages.Length == 0)
        {
            return;
        }

        var domainById = messages.ToDictionary(keySelector: m => m.Id);

        var ids = domainById.Keys.ToArray();

        var dbMessages = await _dbContext.OutboxMessages
            .Where(m => ids.Contains(m.Id))
            .ToArrayAsync(cancellationToken);

        foreach (var dbMessage in dbMessages)
        {
            var domainMessage = domainById[dbMessage.Id];

            dbMessage.Status = domainMessage.Status;
            dbMessage.Error = domainMessage.Error;
        }
    }

    public async Task<OutboxMessage[]> GetPendingAsync(int maxCount, CancellationToken cancellationToken)
    {
        var messages = await _dbContext.OutboxMessages
            .Where(m => m.Status == OutboxMessageStatus.Pending)
            // TODO: uncomment
            // System.NotSupportedException:
            // SQLite does not support expressions of type 'DateTimeOffset' in ORDER BY clauses.
            // Convert the values to a supported type, or use LINQ to Objects to order the results on the client side.
            // .OrderBy(m => m.CreatedAt)
            .Take(maxCount)
            .Select(m => OutboxMessage.FromExisting(m.Id, m.CreatedAt, m.EventType, m.Payload, m.Status, m.Error))
            .ToArrayAsync(cancellationToken);

        return messages;
    }
}