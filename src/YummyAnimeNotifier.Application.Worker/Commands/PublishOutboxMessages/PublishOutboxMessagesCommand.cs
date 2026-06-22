using MediatR;

namespace YummyAnimeNotifier.Application.Worker.Commands.PublishOutboxMessages;

public record PublishOutboxMessagesCommand() : IRequest<Unit>;