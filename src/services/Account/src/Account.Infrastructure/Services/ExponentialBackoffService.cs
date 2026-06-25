using Account.Application.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.SharedKernel.Common.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Singleton)]
public class ExponentialBackoffService : IExponentialBackoffService
{
    private readonly ILogger<ExponentialBackoffService> _logger;
    
    private readonly Random _random = new Random();

    public ExponentialBackoffService(ILogger<ExponentialBackoffService> logger)
    {
        _logger = logger;
    }
    
    public async Task<T?> RetryWithBackoff<T>(Func<Task<T>> func, BackOffOptions? options = null, CancellationToken cancellationToken = default)
    {
        (int maxRetries, int initialDelay, int maxDelay, int timeMultiple) = options ?? new BackOffOptions();

        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                T result = await func();

                _logger.LogInformation(
                    "Operation succeeded after retry count {attempts}/{maxRetries} retries.",
                    attempts,
                    maxRetries);

                return result;

            } catch
            {
                int delay = CalculateDelay(options ?? new BackOffOptions(maxRetries, initialDelay, maxDelay, timeMultiple), attempts);

                await Task.Delay(delay, cancellationToken);

                attempts++;

                _logger.LogError(
                    "Operation failed after retry count {attempts}/{maxRetries} and {delay}ms.",                    
                    attempts,
                    maxRetries,
                    delay);
            }
        }

        _logger.LogError("Max retries reached. Operation ultimately failed.");

        return default(T);
    }

    public async Task RetryWithBackoff(Func<Task> func, BackOffOptions? options = null, CancellationToken cancellationToken = default)
    {
        (int maxRetries, int initialDelay, int maxDelay, int timeMultiple) = options ?? new BackOffOptions();

        int attempts = 0;

        while (attempts < maxRetries)
        {
            try
            {
                await func();

                _logger.LogInformation(
                    "Operation succeeded after retry count {attempts}/{maxRetries} retries.",
                    attempts,
                    maxRetries);

                return;
            }
            catch
            {
                int delay = CalculateDelay(options ?? new BackOffOptions(maxRetries, initialDelay, maxDelay, timeMultiple), attempts);

                await Task.Delay(delay, cancellationToken);

                attempts++;

                _logger.LogError(
                    "Operation failed after retry count {attempts}/{maxRetries} and {delay}ms.",
                    attempts,
                    maxRetries,
                    delay);
            }
        }

        _logger.LogError("Max retries reached. Operation ultimately failed.");
    }
    
    private int CalculateDelay(BackOffOptions options, int attempts)
    {
        (_, int initialDelay, int maxDelay, int timeMultiple) = options;

        double waitTime = Math.Min(initialDelay * Math.Pow(timeMultiple, attempts), maxDelay);

        return (int)Math.Floor(_random.NextDouble() * waitTime);
    }
}