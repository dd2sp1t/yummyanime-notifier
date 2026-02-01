using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public record CancelSubscriptionsCommand(Guid AnimeId) : IRequest<Unit>;