using Account.Infrastructure.Common.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Common.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectAttribute : Attribute
{
    public InjectAttribute(
        ServiceKind serviceKind = ServiceKind.Service,
        ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
        Type? optionType = null)
    {
        ServiceKind = serviceKind;
        ServiceLifetime =  serviceLifetime;
        OptionType = optionType;
    }

    public ServiceKind ServiceKind { get; }

    public ServiceLifetime ServiceLifetime { get; }

    public Type? OptionType { get; }
}