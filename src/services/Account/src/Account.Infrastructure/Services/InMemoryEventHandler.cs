using Account.Application.Contracts;
using Account.Domain.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Services;

[Inject(ServiceKind.Service,ServiceLifetime.Singleton)]
public class InMemoryEventHandler : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryEventHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task DispatchAsync(IDomainEvent @event, CancellationToken cancellationToken = default)
    {
        // Getting the handler type base on the event type
        Type handlerType = typeof(IEventHandler<>) // IEventHandler<EVENT_NAME>
            .MakeGenericType(@event.GetType());

        // Getting handlers register in the di pool base on the handler type
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        { 
            // Invoke the handle method with the event and cancellation token
           await (Task)handlerType
                    .GetMethod("Handle")!
                    .Invoke(handler, new object[] { @event, cancellationToken })!;
        }
    }
}