using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;

public record GetUserSubscriptionsQuery(long TelegramUserId) :
    IRequest<IReadOnlyCollection<GetUserSubscriptionsQueryResultItem>>;