using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Notifications.Commands.NotifyUsers;

public record NotifyUsersCommand(Guid ReleaseId) : IRequest<Unit>;