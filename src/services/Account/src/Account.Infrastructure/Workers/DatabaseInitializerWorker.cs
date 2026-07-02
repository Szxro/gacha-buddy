using Account.Application.Contracts;
using Account.Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Account.Infrastructure.Workers;

public class DatabaseInitializerWorker : BaseWorker<DatabaseInitializerWorker>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public DatabaseInitializerWorker(
        ILogger<BaseWorker<DatabaseInitializerWorker>> logger,
        IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        IDatabaseInitializerService initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializerService>();
        
        await initializer.CanConnectAsync(cancellationToken);

        await initializer.MigrateAsync(cancellationToken);
    }
}