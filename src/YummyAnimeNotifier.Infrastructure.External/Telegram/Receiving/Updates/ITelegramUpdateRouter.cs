using Telegram.Bot.Types;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving.Updates;

internal interface ITelegramUpdateRouter
{
    Task RouteAsync(Update update, CancellationToken cancellationToken = default);
}