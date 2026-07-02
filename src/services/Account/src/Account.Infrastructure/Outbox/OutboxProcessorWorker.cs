using Account.Application.Contracts;
using Account.Infrastructure.Common;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Outbox;

public class OutboxProcessorWorker : BaseWorker<OutboxProcessorWorker>
{
    // Get the expiration time or timeout from configuration?
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(5);
    
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public OutboxProcessorWorker(
        ILogger<BaseWorker<OutboxProcessorWorker>> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using PeriodicTimer periodicTimer = new PeriodicTimer(Interval);

        while (await periodicTimer.WaitForNextTickAsync(cancellationToken) 
               && !cancellationToken.IsCancellationRequested) 
        {
            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            
            IOutboxProcessor processor = scope.ServiceProvider.GetRequiredService<IOutboxProcessor>();
            
            int processedMessages = await processor.ExecuteAsync(cancellationToken);
            
            _logger.LogInformation(
                "Outbox batch processed. Count: {ProcessedMessages}",
                processedMessages);
        }
    }
}