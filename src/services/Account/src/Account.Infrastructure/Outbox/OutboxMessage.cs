namespace Account.Infrastructure.Outbox;

public class OutboxMessage
{
    public int Id { get; init; }

    public required string Type { get; init; }

    public required string Payload { get; init; }

    public string? Error { get; set; }

    public int RetryCount { get; set; }

    public bool WasSent { get; set; }

    public DateTime OccuredOnUtc { get; init; }

    public DateTime? ProcessedOnUtc { get; set; }

    public DateTime? NextRetryOnUtc { get; init; }

    public DateTime? FirstFailedOnUtc  { get; set; }
    
    public DateTime? LastFailedOnUtc  { get; set; }
}