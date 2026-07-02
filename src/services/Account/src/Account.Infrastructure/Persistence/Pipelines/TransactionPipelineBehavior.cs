using Account.Application.Common.Abstractions;
using Account.SharedKernel.Common.Primitives;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Persistence.Pipelines;

public class TransactionPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    private readonly ILogger<TransactionPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly AppDbContext _dbContext;

    public TransactionPipelineBehavior(
        ILogger<TransactionPipelineBehavior<TRequest, TResponse>> logger,
        AppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }
    
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        string commandName = typeof(TRequest).Name;

        try
        {
            TResponse response = await next(cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);

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
            
            await transaction.RollbackAsync(cancellationToken);
            
            throw;
        }
    }
}