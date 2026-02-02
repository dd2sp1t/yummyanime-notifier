using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Infrastructure.Persistence.Entities
{
    public class DbOutboxMessage
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public OutboxMessageStatus Status { get; set; }
        public string Error { get; set; }
    }
}