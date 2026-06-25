using Account.Domain.Common;
using Account.Domain.Contracts;
using Account.Infrastructure.Channels;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Account.Infrastructure.Persistence.Interceptors;

[Inject(ServiceKind.Interceptor)]
public class DomainEventDispatcherInterceptor : SaveChangesInterceptor
{
    private readonly DomainEventChannel _eventChannel;

    public DomainEventDispatcherInterceptor(DomainEventChannel eventChannel)
    {
        _eventChannel = eventChannel;
    }
    
    // when the transaction is completed is going to be executed 
    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        await DispatchDomainEvent(eventData.Context);
        
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async Task DispatchDomainEvent(DbContext? context)
    {
        if (context is null) return;

        List<EntityEntry<Entity>> entities = context
            .ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvent.Count != 0)
            .ToList();
        
        if(entities.Count == 0) return;

        foreach (EntityEntry<Entity> entity in entities)
        {
            List<IDomainEvent> events = entity.Entity.DomainEvent.ToList();

            foreach (IDomainEvent @event in events)
            {
                await _eventChannel.AddEventAsync(@event);
            }

            entity.Entity.ClearEvents();
        }
    }
}