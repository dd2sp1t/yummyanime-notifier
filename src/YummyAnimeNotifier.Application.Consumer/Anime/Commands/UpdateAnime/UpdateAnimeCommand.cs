using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnime;

public record UpdateAnimeCommand(Guid ReleaseId) : IRequest<Unit>;