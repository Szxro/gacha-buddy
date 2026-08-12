using System.Text.Json;
using Account.Application.Contracts;
using Account.Domain.Common;
using Account.Domain.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Persistence.Interceptors;

[Inject(ServiceKind.Interceptor)]
public class OutboxMessageInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _serviceScopeFactory;


    public OutboxMessageInterceptor(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
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
        using IServiceScope scope =  _serviceScopeFactory.CreateScope();

        IEventTypeResolver eventResolver = scope.ServiceProvider.GetRequiredService<IEventTypeResolver>();
        
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

        List<OutboxMessage> messages = events.Select(@event =>
        {
            Type? eventType = eventResolver.Resolve(@event.GetType().Name);
            
            if(eventType is null)  throw new InvalidOperationException($"Could not resolve event type {@event.GetType().Name}");
            
            return new OutboxMessage
            {
                Type = @event.GetType().Name,
                Payload = JsonSerializer.Serialize(@event, eventType),
                OccuredOnUtc = DateTime.UtcNow,
                RetryCount = 0
            };
        }).ToList();
        
        context.Set<OutboxMessage>().AddRange(messages);
        
        return Task.CompletedTask;
    }
}