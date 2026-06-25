using Account.Domain.Contracts;

namespace Account.Application.Contracts;

public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent @event,CancellationToken cancellationToken = default);
}