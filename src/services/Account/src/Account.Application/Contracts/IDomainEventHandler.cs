using MediatR;

namespace Account.SharedKernel.Contracts;

public interface IDomainEventHandler<in TNotification> : INotificationHandler<TNotification>
    where TNotification : IDomainEvent
{ }