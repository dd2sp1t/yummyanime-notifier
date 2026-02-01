using AniMediaNotifier.Application.AniMedia;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace AniMediaNotifier.Application.Subscriptions.Commands.SubscribeToAnime;

public class SubscribeToAnimeCommandValidator : AbstractValidator<SubscribeToAnimeCommand>
{
    public SubscribeToAnimeCommandValidator(IOptions<AniMediaSiteData> options)
    {
        var siteData = options.Value;

        RuleFor(command => command.TelegramUserId)
            .GreaterThan(0)
            .WithMessage("TelegramUserId must be greater than 0");

        RuleFor(command => command.SourceLink)
            .NotEmpty()
                .WithMessage("SourceLink is required")
            .Must(link => Uri.TryCreate(link, UriKind.Absolute, out _))
                .WithMessage("SourceLink must be an absolute URI")
            .Must(link => link.Contains("/episode/") == false)
                .WithMessage("SourceLink must not point to a specific episode")
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
                .WithMessage($"SourceLink must belong to the '{siteData.Domain}' domain"); ;
    }
}