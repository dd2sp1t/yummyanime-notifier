using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.UnsubscribeFromAnime;

public record UnsubscribeFromAnimeCommand(
    long TelegramUserId,
    string RuName
) : IRequest<Unit>;
