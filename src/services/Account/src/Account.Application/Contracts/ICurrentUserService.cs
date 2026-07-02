namespace Account.Application.Contracts;

public interface ICurrentUserService
{
    string? GetCurrentUserName();
}