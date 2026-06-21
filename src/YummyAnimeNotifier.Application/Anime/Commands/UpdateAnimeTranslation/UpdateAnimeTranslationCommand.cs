using MediatR;

namespace YummyAnimeNotifier.Application.Anime.Commands.UpdateAnimeTranslation;

public record UpdateAnimeTranslationCommand(Guid ReleaseId) : IRequest<Unit>;