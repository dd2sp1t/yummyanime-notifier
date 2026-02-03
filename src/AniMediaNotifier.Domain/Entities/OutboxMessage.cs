using System.Text.Json;
using AniMediaNotifier.Domain.Enums;
using AniMediaNotifier.Domain.Events;

namespace AniMediaNotifier.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public string EventType { get; private set; }
        public string Payload { get; private set; }
        public OutboxMessageStatus Status { get; private set; }
        public string Error { get; private set; }

        public bool TryDeserializeEvent(out Event @event, out string error)
        {
            @event = null;
            error = null;

            var type = Type.GetType(EventType);
            if (type is null)
            {
                error = $"Unknown event type {EventType}";
                return false;
            }

            @event = JsonSerializer.Deserialize(Payload, type) as Event;
            if (@event is null)
            {
                error = $"Failed to deserialize event {EventType}";
                return false;
            }

            return true;
        }

        public void MarkAsPublished()
        {
            Status = OutboxMessageStatus.Published;
        }

        public void MarkAsFailed(string error)
        {
            Status = OutboxMessageStatus.Failed;
            Error = error;
        }

        public static OutboxMessage Create<TEvent>(TEvent @event) where TEvent : Event
        {
            return new OutboxMessage
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                EventType = @event.GetType().AssemblyQualifiedName,
                Payload = JsonSerializer.Serialize(@event),
                Status = OutboxMessageStatus.Pending
            };
        }

        public static OutboxMessage FromExisting(
            Guid id,
            DateTimeOffset createdAt,
            string eventType,
            string payload,
            OutboxMessageStatus status,
            string error)
        {
            return new OutboxMessage
            {
                Id = id,
                CreatedAt = createdAt,
                EventType = eventType,
                Payload = payload,
                Status = status,
                Error = error
            };
        }
    }
}