using System.Data;
using Account.Application.Common.Abstractions;
using Account.SharedKernel.Common.Primitives;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Account.Application.Common.Pipelines;

public class RequestTransactionPipelineBehavior<TRequest, TResponse> 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<RequestTransactionPipelineBehavior<TRequest, TResponse>> _logger;

    public RequestTransactionPipelineBehavior(
        IAppDbContext dbContext,
        ILogger<RequestTransactionPipelineBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using IDbTransaction transaction = await _dbContext.GetDbTransaction(cancellationToken);
        
        string commandName = typeof(TRequest).Name;
        
        try
        {
            TResponse response = await next(cancellationToken);
            
            await _dbContext.SaveChangesAsync(cancellationToken);
            
            transaction.Commit();
            
            _logger.LogInformation(
                "Command {CommandName} executed successfully. Transaction committed.",
                commandName);

            return response;
        }
        catch
        {
            _logger.LogError(
                "An unexpected error happen while trying to complete the command {commandName}, rolling back the transaction.",
                commandName);
            
            transaction.Rollback();
            
            throw;
        }
    }
}