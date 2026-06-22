using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnimeTranslation;

public record UpdateAnimeTranslationCommand(Guid ReleaseId) : IRequest<Unit>;