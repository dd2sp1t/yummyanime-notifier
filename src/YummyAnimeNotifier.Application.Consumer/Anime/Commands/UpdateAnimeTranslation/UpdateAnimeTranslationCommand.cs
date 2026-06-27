using MediatR;
using YummyAnimeNotifier.Application.PipelineBehaviors;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.UpdateAnimeTranslation;

public record UpdateAnimeTranslationCommand(Guid ReleaseId) : IRequest<Unit>, IConcurrencyProtectedRequest;