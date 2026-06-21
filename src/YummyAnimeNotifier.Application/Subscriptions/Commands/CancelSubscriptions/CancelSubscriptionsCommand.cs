using MediatR;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public record CancelSubscriptionsCommand(
    Guid AnimeId,
    Guid TranslationSourceId
) : IRequest<Unit>;