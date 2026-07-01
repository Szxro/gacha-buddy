namespace Account.SharedKernel.Common.Options;

public record BackOffOptions(
    int maxRetries = 3,
    int initialDelay = 1000,
    int maxDelay = 5000,
    int timeMultiple = 2);