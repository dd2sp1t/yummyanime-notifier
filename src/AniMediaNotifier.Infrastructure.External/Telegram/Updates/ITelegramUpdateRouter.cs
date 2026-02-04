using Telegram.Bot.Types;

namespace AniMediaNotifier.Infrastructure.External.Telegram.Updates;

internal interface ITelegramUpdateRouter
{
    Task RouteAsync(Update update, CancellationToken cancellationToken = default);
}