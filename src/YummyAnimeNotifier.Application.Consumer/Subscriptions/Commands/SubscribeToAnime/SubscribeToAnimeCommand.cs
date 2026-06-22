using MediatR;
using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Consumer.Subscriptions.Commands.SubscribeToAnime;

public record SubscribeToAnimeCommand(
    long TelegramUserId,
    string SourceLink,
    TranslationType TranslationType,
    string TranslationSourceName
) : IRequest<Unit>;
