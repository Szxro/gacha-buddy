using System.Diagnostics;
using System.Text.Json;
using Account.Application.Common.Abstractions;
using Account.Application.Contracts;
using Account.Domain.Entities;
using Account.SharedKernel.Common.Primitives;
using MediatR;

namespace Account.Application.Common.Pipelines;

public class RequestAuditLogPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuditableRequest
    where TResponse : Result
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IAuditLogRepository _auditLogRepository;

    public RequestAuditLogPipelineBehavior(
        ICurrentUserService currentUserService,
        IAuditLogRepository  auditLogRepository)
    {
        _currentUserService = currentUserService;
        _auditLogRepository = auditLogRepository;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        
        TResponse response = await next(cancellationToken);
        
        stopwatch.Stop();
        
        AuditLog auditLog = new AuditLog
        {
            UserId = _currentUserService.UserId,
            RequestName = typeof(TRequest).Name,
            RequestData = JsonSerializer.Serialize(request.GetAuditData()),
            ResourceName = request.ResourceName,
            ResourceId = request.ResourceId,
            IpAddress =  _currentUserService.IpAddress,
            UserAgent =  _currentUserService.UserAgent,
            ErrorMessage = response.IsFailure 
                ? response.Error.Description 
                : null,
            IsSuccessful = response.IsSuccess,
            ExecutionTimeInMs =  stopwatch.ElapsedMilliseconds,
            CreatedAt =  DateTime.UtcNow,
        };
        
        _auditLogRepository.Add(auditLog);

        return response;
    }
}