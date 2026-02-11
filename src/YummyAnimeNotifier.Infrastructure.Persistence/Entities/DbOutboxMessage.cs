using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Infrastructure.Persistence.Entities
{
    public class DbOutboxMessage
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string EventType { get; set; }
        public string Payload { get; set; }
        public OutboxMessageStatus Status { get; set; }
        public string Error { get; set; }
    }
}