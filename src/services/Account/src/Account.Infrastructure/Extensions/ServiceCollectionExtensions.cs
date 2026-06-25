using System.Reflection;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Account.Infrastructure.Options.Validations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Account.Infrastructure.Extensions;

public static partial class InfrastructureExtensions
{
    public static IServiceCollection RegisterServicesFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        IEnumerable<TypeInfo> types = assembly.DefinedTypes
            .Where(x => x.GetCustomAttribute<InjectAttribute>() is not null && x is { IsInterface: false, IsAbstract: false, IsClass: true });

        foreach (TypeInfo type in types)
        {
            InjectAttribute? attribute = type.GetCustomAttribute<InjectAttribute>();
            
            // The class itself is going to be the interfaceType or the interface that have the convention I{TYPENAME}
            Type serviceType = type.GetInterfaces().FirstOrDefault() ?? type;
            
            if (attribute is null) continue;

            switch (attribute.ServiceKind)
            {
                // It can be merged the cases in the switch expression
                case ServiceKind.Service: 
                case ServiceKind.Repository:
                    services.RegisterDependency(serviceType, type,  attribute.ServiceLifetime);
                    break;
                case ServiceKind.Interceptor:
                    // By default, the interceptors have an interface call ISaveChangesInterceptor
                    services.AddSingleton(type);
                    break;
                case ServiceKind.Options:
                    services.ConfigureOptions(type);
                    
                    if (attribute.OptionType is not null)
                    {
                        services.AddFluentValidator(attribute.OptionType);
                    }
                    
                    break;
                case ServiceKind.Worker:
                    services.AddSingleton(typeof(IHostedService), type);
                    break;
            }
        }

        return services;
    }

    private static IServiceCollection RegisterDependency(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime serviceLifetime)
    {
        // Represents the service that is injected into the ServiceCollection
        ServiceDescriptor descriptor = new ServiceDescriptor(serviceType, implementationType, serviceLifetime);
        
        services.Add(descriptor);
        
        return services;
    }
    
    
    private static IServiceCollection AddFluentValidator(
        this IServiceCollection services,
        Type optionType)
    {
        Type validatorType = typeof(IValidateOptions<>)
            .MakeGenericType(optionType);

        Type fluentValidatorType = typeof(FluentOptionsValidator<>)
            .MakeGenericType(optionType);

        services.AddSingleton(
            validatorType,
            provider =>
            {
                IServiceScopeFactory scopeFactory =
                    provider.GetRequiredService<IServiceScopeFactory>();
                //  Create an instance of a type without having to declare it explicitly in code.
                return Activator.CreateInstance(
                    fluentValidatorType,
                    scopeFactory)!;
            });

        return services;
    }
}