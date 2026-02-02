namespace AniMediaNotifier.Domain.Enums;

public enum OutboxMessageStatus
{
    None,
    Pending,
    Published,
    Failed
}