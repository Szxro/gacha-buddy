using System.Reflection;
using Account.Application.Contracts;
using Account.Domain.Contracts;

namespace Account.Application.Common.Resolvers;

public class EventTypeResolver : IEventTypeResolver
{
    private static readonly Dictionary<string, Type> TypeCache = new();

    static EventTypeResolver()
    {
        Assembly domainAssembly = typeof(IDomainEvent).Assembly;
        
        var eventTypes = domainAssembly.GetTypes()
            .Where(t => typeof(IDomainEvent).IsAssignableFrom(t) & !t.IsAbstract & !t.IsInterface); // class_name : IDomainEvent

        foreach (var eventType in eventTypes)
        {
            TypeCache.Add(eventType.Name, eventType);
        }
    }

    public Type? Resolve(string typeName)
    {
        return TypeCache.TryGetValue(typeName, out Type? type) ? type : null;
    }
}