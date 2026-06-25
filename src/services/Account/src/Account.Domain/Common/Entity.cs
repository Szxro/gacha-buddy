using System.ComponentModel.DataAnnotations.Schema;
using Account.Domain.Contracts;

namespace Account.Domain.Common;

public abstract class Entity
{
    public int Id { get; set; }

    private List<IDomainEvent> domainEvents = [];
    
    [NotMapped]
    public IReadOnlyCollection<IDomainEvent> DomainEvent => domainEvents;

    public void AddEvent(IDomainEvent @event)
    {
        domainEvents.Add(@event);
    }

    public void RemoveEvent(IDomainEvent @event)
    {
        domainEvents.Remove(@event);
    }

    public void ClearEvents()
    {
        domainEvents.Clear();
    }
}