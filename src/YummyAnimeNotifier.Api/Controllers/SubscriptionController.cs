using YummyAnimeNotifier.Api.Requests;
using YummyAnimeNotifier.Application.Subscriptions.Commands.SubscribeToAnime;
using YummyAnimeNotifier.Application.Subscriptions.Commands.UnsubscribeFromAnime;
using YummyAnimeNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;
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
        var command = new SubscribeToAnimeCommand(request.TelegramUserId, request.SourceLink);

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpPost("/unsubscribe")]
    public async Task<IActionResult> UnsubscribeFromAnimeAsync(
        [FromBody] UnsubscribeFromAnimeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UnsubscribeFromAnimeCommand(request.TelegramUserId, request.RuName);

        await _mediator.Send(command, cancellationToken);

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserSubscriptionsAsync(
        [FromQuery] long telegramUserId,
        CancellationToken cancellationToken)
    {
        var query = new GetUserSubscriptionsQuery(telegramUserId);

        var subscriptions = await _mediator.Send(query, cancellationToken);

        return Ok(subscriptions);
    }
}