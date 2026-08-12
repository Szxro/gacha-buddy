namespace Account.Application.Common.Abstractions;

public interface IAuditableRequest
{
    public string? ResourceName { get; }
    
    public string? ResourceId { get; }

    object? GetAuditData();
}