using Telegram.Bot.Types;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Updates.Handlers;

internal interface ITelegramUpdateHandler
{
    bool CanHandle(Update update);
    Task HandleAsync(Update update, CancellationToken cancellationToken = default);
}