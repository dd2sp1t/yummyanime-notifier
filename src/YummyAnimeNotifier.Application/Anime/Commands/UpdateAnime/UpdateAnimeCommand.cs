using MediatR;

namespace YummyAnimeNotifier.Application.Anime.Commands.UpdateAnime;

public record UpdateAnimeCommand(Guid ReleaseId) : IRequest<Unit>;