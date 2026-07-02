using Account.Application.Common.Pipelines;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
    {
        // stops executing a validator class as soon as a rule fails
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
        
        services.AddMediatR(options =>
        {
            string? licenseKey = Environment.GetEnvironmentVariable("MEDIATR_LICENSE_KEY");

            options.LicenseKey = licenseKey;
            
            options.RegisterServicesFromAssembly(typeof(ApplicationServiceRegistration).Assembly);
            
            // Pipelines
            options.AddOpenBehavior(typeof(RequestValidationPipelineBehavior<,>));
            options.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
            options.AddOpenBehavior(typeof(RequestPerformancePipelineBehavior<,>));
            options.AddOpenBehavior(typeof(RequestExceptionHandlingPipelineBehavior<,>));
        });
        
        return services;
    }
}