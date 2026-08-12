using Account.Application.Contracts;
using Account.SharedKernel.Common.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Common.Pipelines;

public class RequestLoggingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestLoggingPipelineBehavior(
        ICurrentUserService currentUserService,
        ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        _logger.LogInformation(
            "Processing the request {RequestName} requested by the user {User}",
            requestName,
            _currentUserService.UserName ?? "System");

        TResponse response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            _logger.LogInformation(
                "Completed the request {RequestName} requested by the user {User}",
                requestName,
                _currentUserService.UserName ?? "System");

            return response;
        }

        _logger.LogWarning(
            "Completed the request {RequestName} requested by the user {User} with an error of type {ErrorName}",
            requestName,
            _currentUserService.UserName ?? "System",
            response.Error.ErrorCode);

        return response;
    }
}