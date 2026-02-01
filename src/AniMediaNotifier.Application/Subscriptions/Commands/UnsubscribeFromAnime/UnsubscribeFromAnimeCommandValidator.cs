using FluentValidation;

namespace AniMediaNotifier.Application.Subscriptions.Commands.UnsubscribeFromAnime;

public class UnsubscribeFromAnimeCommandValidator : AbstractValidator<UnsubscribeFromAnimeCommand>
{
    public UnsubscribeFromAnimeCommandValidator()
    {
        RuleFor(x => x.TelegramUserId)
            .GreaterThan(0)
            .WithMessage("TelegramUserId must be greater than 0");

        RuleFor(x => x.RuName)
            .NotEmpty()
            .WithMessage("RuName is required");
    }
}