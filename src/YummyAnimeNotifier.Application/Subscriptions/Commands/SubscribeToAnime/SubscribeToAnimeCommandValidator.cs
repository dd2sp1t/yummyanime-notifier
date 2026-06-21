using FluentValidation;
using Microsoft.Extensions.Options;
using YummyAnimeNotifier.Application.YummyAnime;
using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.SubscribeToAnime;

public class SubscribeToAnimeCommandValidator : AbstractValidator<SubscribeToAnimeCommand>
{
    private const int SourceLinkMaxLength = 500;
    private const int TranslationSourceNameMaxLength = 255;

    public SubscribeToAnimeCommandValidator(IOptions<YummyAnimeSiteData> options)
    {
        var siteData = options.Value;

        RuleFor(command => command.TelegramUserId)
            .GreaterThan(0)
            .WithMessage("TelegramUserId must be greater than 0");

        RuleFor(command => command.SourceLink)
            .NotEmpty()
                .WithMessage("SourceLink is required")
            .MaximumLength(SourceLinkMaxLength)
                .WithMessage($"SourceLink must not exceed {SourceLinkMaxLength} characters")
            // TODO: move all parts to BeValidAnimeSourceLink() ??
            .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _))
                .WithMessage("SourceLink must be an absolute URI")
            .Must(link => link.Contains("/catalog/item/"))
                .WithMessage("SourceLink must point to a specific catalog item")
            .Must(link =>
            {
                if (Uri.TryCreate(link, UriKind.Absolute, out var parsed) == false)
                {
                    return false;
                }

                if (Uri.TryCreate(siteData.Domain, UriKind.Absolute, out var allowed) == false)
                {
                    return false;
                }

                return string.Equals(
                    parsed.Host,
                    allowed.Host,
                    StringComparison.OrdinalIgnoreCase);
            })
                .WithMessage($"SourceLink must belong to the '{siteData.Domain}' domain");

        RuleFor(command => command.TranslationType)
            .NotEqual(TranslationType.None)
                .WithMessage("TranslationType must be specified")
            .IsInEnum()
                .WithMessage("TranslationType must be a valid enum value");

        RuleFor(command => command.TranslationSourceName)
            .NotEmpty()
                .WithMessage("TranslationSourceName is required")
            .MaximumLength(TranslationSourceNameMaxLength)
                .WithMessage($"TranslationSourceName must not exceed {TranslationSourceNameMaxLength} characters");
    }
}