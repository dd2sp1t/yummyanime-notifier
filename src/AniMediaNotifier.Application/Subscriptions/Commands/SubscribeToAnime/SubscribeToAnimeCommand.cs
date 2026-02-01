using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.SubscribeToAnime;

public record SubscribeToAnimeCommand(
    long TelegramUserId,
    string SourceLink
) : IRequest<Unit>;
