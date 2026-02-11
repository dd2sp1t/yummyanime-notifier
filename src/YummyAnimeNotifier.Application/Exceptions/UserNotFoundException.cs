namespace YummyAnimeNotifier.Application.Exceptions;

public class UserNotFoundException : ApplicationException
{
    public long TelegramUserId { get; }

    public UserNotFoundException(long telegramUserId)
        : base("The user was not found")
    {
        TelegramUserId = telegramUserId;
    }
}