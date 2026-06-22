using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Subscriptions.Commands.CancelSubscriptions;

public record CancelSubscriptionsCommand(
    Guid AnimeId,
    Guid TranslationSourceId
) : IRequest<Unit>;