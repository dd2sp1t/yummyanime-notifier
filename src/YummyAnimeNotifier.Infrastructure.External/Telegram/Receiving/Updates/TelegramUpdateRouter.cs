using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving.Updates.Handlers;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving.Updates;

internal sealed class TelegramUpdateRouter : ITelegramUpdateRouter
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TelegramUpdateRouter(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task RouteAsync(Update update, CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetRequiredService<IEnumerable<ITelegramUpdateHandler>>();

        foreach (var handler in handlers)
        {
            if (handler.CanHandle(update))
            {
                await handler.HandleAsync(update, cancellationToken);
                return;
            }
        }
    }
}