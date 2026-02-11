using MediatR;

namespace YummyAnimeNotifier.Application.Notifications.Commands.NotifyUsers;

public record NotifyUsersCommand(Guid ReleaseId) : IRequest<Unit>;