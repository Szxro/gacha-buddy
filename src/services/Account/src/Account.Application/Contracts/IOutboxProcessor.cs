namespace Account.Application.Contracts;

public interface IOutboxProcessor
{
    Task<int> ExecuteAsync(CancellationToken cancellationToken);
}