using MediatR;

namespace YummyAnimeNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;

public record GetUserSubscriptionsQuery(long TelegramUserId) :
    IRequest<IReadOnlyCollection<GetUserSubscriptionsQueryResultItem>>;