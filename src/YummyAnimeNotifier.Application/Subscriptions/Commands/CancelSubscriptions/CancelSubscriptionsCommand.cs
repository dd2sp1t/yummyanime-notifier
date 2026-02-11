using MediatR;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public record CancelSubscriptionsCommand(Guid AnimeId) : IRequest<Unit>;