using Account.Infrastructure.Extensions;
using Account.Infrastructure.Options;
using Account.Infrastructure.Persistence;
using Account.Infrastructure.Persistence.Interceptors;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Account.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IHostEnvironment  environment)
    {
        services.RegisterServicesFromAssembly(typeof(InfrastructureServiceRegistration).Assembly);
        
        services.AddValidatorsFromAssembly(typeof(InfrastructureServiceRegistration).Assembly);
        
        services.AddDbContext<AppDbContext>((provider, options) =>
        {
            DatabaseOptions databaseOptions = provider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            
            options.UseSqlServer(databaseOptions.ConnectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(databaseOptions.CommandTimeout);
                
            })
            .AddInterceptors(provider.GetRequiredService<OutboxMessageInterceptor>())
            .UseSnakeCaseNamingConvention();

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });
        
        return services;
    }
}