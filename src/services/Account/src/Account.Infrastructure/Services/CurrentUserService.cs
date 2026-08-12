using System.IdentityModel.Tokens.Jwt;
using Account.Application.Contracts;
using Account.Infrastructure.Common.Attributes;
using Account.Infrastructure.Common.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Account.Infrastructure.Services;

[Inject(ServiceKind.Service, ServiceLifetime.Scoped)]
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContext;

    public CurrentUserService(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;
    }
    
    public string? UserName 
        => _httpContext?.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name)?.Value;

    public int? UserId
    {
        get
        {
            string? userIdClaim = _httpContext.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub)?.Value;

            return int.TryParse(userIdClaim, out int userId)
                ? userId
                : null;
        }
    }

    public string IpAddress =>
        _httpContext?.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    public string UserAgent => 
        _httpContext?.HttpContext?.Request.Headers.UserAgent.ToString() ??  "unknown";
}