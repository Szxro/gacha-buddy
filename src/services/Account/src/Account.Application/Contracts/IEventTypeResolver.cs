namespace Account.Application.Contracts;

public interface IEventTypeResolver
{
    Type? Resolve(string typeName);
}