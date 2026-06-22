using YummyAnimeNotifier.Api.Requests;
using YummyAnimeNotifier.Application.Consumer.Subscriptions.Commands.SubscribeToAnime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace YummyAnimeNotifier.Api.Controllers;

[ApiController]
[Route("subscriptions")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;

    public SubscriptionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("/subscribe")]
    public async Task<IActionResult> SubscribeToAnimeAsync(
        [FromBody] SubscribeToAnimeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new SubscribeToAnimeCommand(
            request.TelegramUserId,
            request.SourceLink,
            request.TranslationType,
            request.TranslationSourceName);

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }
}