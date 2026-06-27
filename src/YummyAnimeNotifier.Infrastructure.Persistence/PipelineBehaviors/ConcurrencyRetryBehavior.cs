using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.PipelineBehaviors;

namespace YummyAnimeNotifier.Infrastructure.Persistence.PipelineBehaviors;

public class ConcurrencyRetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IConcurrencyProtectedRequest
{
    private const int MaxRetries = 3;
    private readonly ILogger<ConcurrencyRetryBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public ConcurrencyRetryBehavior(
        ILogger<ConcurrencyRetryBehavior<TRequest, TResponse>> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxRetries; attempt++)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) when (attempt < MaxRetries - 1)
            {
                // retry
                _logger.LogWarning(
                   "Concurrency conflict for {RequestType} (attempt {Attempt}/{MaxRetries}). Retrying...",
                   typeof(TRequest).Name,
                   attempt + 1,
                   MaxRetries);

                _unitOfWork.ClearTracking();
            }
        }

        return await next(cancellationToken);
    }
}