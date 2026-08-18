using Account.Domain.Contracts;

namespace Account.Domain.Events;

public class EmailConfirmationEvent : IDomainEvent
{
    public string Email { get; set; } = string.Empty;
    
    public required string Username { get; init; }

    public required string Code { get; init; }
}