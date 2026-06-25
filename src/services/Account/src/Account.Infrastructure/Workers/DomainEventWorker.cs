using Account.Application.Contracts;
using Account.Domain.Contracts;
using Account.Infrastructure.Channels;
using Account.Infrastructure.Common;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.SharedKernel.Common.Options;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Workers;

[Inject(ServiceKind.Worker)]
public class DomainEventWorker : BaseWorker<DomainEventWorker>
{
    private readonly DomainEventChannel _domainEventChannel;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IExponentialBackoffService _exponentialBackoff;

    public DomainEventWorker(
        ILogger<BaseWorker<DomainEventWorker>> logger,
        DomainEventChannel domainEventChannel,
        IEventDispatcher eventDispatcher,
        IExponentialBackoffService exponentialBackoff) : base(logger)
    {
        _domainEventChannel = domainEventChannel;
        _eventDispatcher = eventDispatcher;
        _exponentialBackoff = exponentialBackoff;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        await foreach (IDomainEvent @event in _domainEventChannel.ReadAllAsync(cancellationToken))
        {
            try
            {
                await _eventDispatcher.DispatchAsync(@event, cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogError(
                    "An unexpected error happen while trying to publish the domain event {eventName} with the error message : {message}, retrying...",
                    @event.GetType().Name,
                    ex.Message);
                
                await _exponentialBackoff.RetryWithBackoff(
                    () => _eventDispatcher.DispatchAsync(@event, cancellationToken),                                                        
                    new BackOffOptions(3, 100, 5000, 2),
                    cancellationToken);
            }
        }
    }
}