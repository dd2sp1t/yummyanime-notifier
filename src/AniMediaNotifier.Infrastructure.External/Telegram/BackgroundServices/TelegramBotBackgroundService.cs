using AniMediaNotifier.Infrastructure.External.Telegram.Updates;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AniMediaNotifier.Infrastructure.External.Telegram.BackgroundServices;

internal sealed class TelegramBotBackgroundService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ITelegramUpdateRouter _router;
    private readonly ILogger<TelegramBotBackgroundService> _logger;

    public TelegramBotBackgroundService(
        ITelegramBotClient botClient,
        ITelegramUpdateRouter router,
        ILogger<TelegramBotBackgroundService> logger)
    {
        _botClient = botClient;
        _router = router;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(
            updateHandler: async (bot, update, cancelationToken) =>
            {
                try
                {
                    await _router.RouteAsync(update, cancelationToken);
                }
                catch (Exception exception)
                {
                    _logger.Log(LogLevel.Error, exception, "Failed to process Telegram update");
                }
            },
            errorHandler: (_, exception, _) =>
            {
                _logger.Log(LogLevel.Error, exception, "Telegram polling error");
                return Task.CompletedTask;
            },
            cancellationToken: stoppingToken);
    }
}