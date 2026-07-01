using System.Text.Json;
using Account.Application.Contracts;
using Account.Domain.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Persistence;
using Account.SharedKernel.Common.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Outbox;

[Inject(serviceKind:ServiceKind.Service, ServiceLifetime.Scoped)]
public class OutboxProcessor : IOutboxProcessor
{
    private readonly int BatchSize = 10;
    
    private readonly AppDbContext _context;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IExponentialBackoffService _exponentialBackoffService;

    public OutboxProcessor(
        AppDbContext context,
        IEventDispatcher eventDispatcher,
        IExponentialBackoffService exponentialBackoffService )
    {
        _context = context;
        _eventDispatcher = eventDispatcher;
        _exponentialBackoffService = exponentialBackoffService;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        List<OutboxMessage> messages = await _context.OutboxMessage
            .Where(x => !x.WasSent && x.ProcessedOnUtc == null && x.Error == null)
            .Take(BatchSize)
            .OrderBy(x => x.OccuredOnUtc)
            .ToListAsync(cancellationToken);
        
        foreach (OutboxMessage message in messages)
        {
            IDomainEvent? @event = JsonSerializer.Deserialize<IDomainEvent>(message.Payload);
            
            try
            {
                await _eventDispatcher.DispatchAsync(@event!, cancellationToken);

                message.WasSent = true;
                
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch
            {
                message.FirstFailedOnUtc = DateTime.UtcNow;
                
                await _exponentialBackoffService.RetryWithBackoff(
                    () => _eventDispatcher.DispatchAsync(@event!, cancellationToken),
                    new BackOffOptions(3, 100, 5000, 2),
                    onComplete: (ex) => MarkAsFailedAsync(ex,message),
                    cancellationToken:cancellationToken);
            }
        }
        
        await _context.SaveChangesAsync(cancellationToken);

        return messages.Count;
    }

    private Task MarkAsFailedAsync(Exception? lastException,OutboxMessage message)
    {
        message.Error = lastException?.Message ?? "Unknown Error";

        message.LastFailedOnUtc = DateTime.UtcNow;
        
        message.RetryCount = 3;
        
        return Task.CompletedTask;
    }
}