using YummyAnimeNotifier.Application.Subscriptions.Commands.SubscribeToAnime;
using MediatR;
using Telegram.Bot.Types;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram.Updates.Handlers;

internal sealed class SubscribeCommandHandler : ITelegramUpdateHandler
{
    private readonly IMediator _mediator;

    public SubscribeCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public bool CanHandle(Update update)
    {
        return update.Message?.Text?.StartsWith("/subscribe") == true;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;

        var parts = message.Text.Split(
            separator: ' ',
            count: 2,
            StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2)
        {
            return;
        }

        var link = parts[1];

        var command = new SubscribeToAnimeCommand(
            TelegramUserId: message.From.Id,
            link,
            // TODO: replace default values
            TranslationType: default,
            TranslationSourceName: default);
        await _mediator.Send(command, cancellationToken);
    }
}