using Account.SharedKernel.Common.Options;

namespace Account.Application.Contracts;

public interface IExponentialBackoffService
{
    Task<T?> RetryWithBackoff<T>(Func<Task<T>> func, BackOffOptions? options = null, CancellationToken cancellationToken = default);

    Task RetryWithBackoff(Func<Task> func, BackOffOptions? options = null, CancellationToken cancellationToken = default);
}