namespace AniMediaNotifier.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public long TelegramUserId { get; private set; }

    public static User FromExisting(Guid id, long telegramUserId)
    {
        return new User
        {
            Id = id,
            TelegramUserId = telegramUserId
        };
    }
}