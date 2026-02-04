using AniMediaNotifier.Application.Notifications.Formatters;
using AniMediaNotifier.Application.Notifications.Formatters.Enums;
using AniMediaNotifier.Application.Notifications.Senders;
using AniMediaNotifier.Application.Notifications.Senders.Models;
using AniMediaNotifier.Application.Persistence.Repositories;
using AniMediaNotifier.Domain.Entities;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AniMediaNotifier.Infrastructure.External.Telegram.Notifications;

public class TelegramNotificationSender : INotificationSender
{
    private readonly IUserRepository _userRepository;
    private readonly ITelegramBotClient _botClient;
    private readonly INotificationFormatter _formatter;
    private readonly ILogger<TelegramNotificationSender> _logger;

    public TelegramNotificationSender(
        IUserRepository userRepository,
        ITelegramBotClient botClient,
        INotificationFormatter formatter,
        ILogger<TelegramNotificationSender> logger)
    {
        _userRepository = userRepository;
        _botClient = botClient;
        _formatter = formatter;
        _logger = logger;
    }

    public async Task<SendResult> TrySendAsync(Notification notification, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(notification.UserId, cancellationToken);
        if (user.TelegramUserId.HasValue == false)
        {
            return new SendResult(
                Success: false,
                Error: "User has no linked Telegram account");
        }

        try
        {
            var chatId = new ChatId(user.TelegramUserId.Value);
            var message = _formatter.Format(notification);
            var parseMode = message.Format switch
            {
                NotificationFormat.MarkdownV2 => ParseMode.MarkdownV2,
                NotificationFormat.Html => ParseMode.Html,
                _ => ParseMode.None
            };

            await _botClient.SendMessage(
                chatId,
                message.Text,
                parseMode,
                cancellationToken: cancellationToken);

            return new SendResult(Success: true, Error: null); ;
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                message: "Failed to send Telegram message to user");

            return new SendResult(
                Success: false,
                Error: exception.Message);
        }
    }
}