using Account.Domain.Contracts;

namespace Account.Application.Contracts;

public interface IEventHandler<in TEvent>
    where TEvent : IDomainEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken = default);
}