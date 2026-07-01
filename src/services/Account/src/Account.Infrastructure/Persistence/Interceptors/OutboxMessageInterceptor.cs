using System.Text.Json;
using Account.Domain.Common;
using Account.Domain.Contracts;
using Account.Infrastructure.Channels;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Account.Infrastructure.Persistence.Interceptors;

[Inject(ServiceKind.Interceptor)]
public class OutboxMessageInterceptor : SaveChangesInterceptor
{
    private readonly DomainEventChannel _eventChannel;

    public OutboxMessageInterceptor(DomainEventChannel eventChannel)
    {
        _eventChannel = eventChannel;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await DispatchDomainEvent(eventData.Context);
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private Task DispatchDomainEvent(DbContext? context)
    {
        if (context is null) return Task.CompletedTask;

        List<IDomainEvent> events = context
            .ChangeTracker
            .Entries<Entity>()
            .SelectMany(x => x.Entity.DomainEvent)
            .ToList();
        
        if (events.Count <= 0) return Task.CompletedTask;

        foreach (var entity in context.ChangeTracker.Entries<Entity>())
        {
            entity.Entity.ClearEvents();
        }

        List<OutboxMessage> messages = events.Select(@event => new OutboxMessage
        {
            Type = @event.GetType().Name,
            Payload = JsonSerializer.Serialize(@event),
            OccuredOnUtc =  DateTime.UtcNow,
            RetryCount = 0
        }).ToList();
        
        context.Set<OutboxMessage>().AddRange(messages);
        
        return Task.CompletedTask;
    }
}