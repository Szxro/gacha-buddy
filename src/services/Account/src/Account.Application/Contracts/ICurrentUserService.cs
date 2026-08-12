namespace Account.Application.Contracts;

public interface ICurrentUserService
{
    string? UserName { get; }

    int? UserId { get; }
    
    string IpAddress { get; }
    
    string UserAgent { get; }
}