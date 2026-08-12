using Account.Domain.Contracts;

namespace Account.Domain.Events;

public class WelcomeEvent : IDomainEvent
{
    public required string Username { get; init; } 
}