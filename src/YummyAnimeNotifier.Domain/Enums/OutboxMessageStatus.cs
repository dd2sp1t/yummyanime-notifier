namespace YummyAnimeNotifier.Domain.Enums;

public enum OutboxMessageStatus
{
    None,
    Pending,
    Published,
    Failed
}