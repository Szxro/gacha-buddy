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
    
    public string? GetCurrentUserName()
    {
        return _httpContext?.HttpContext?.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Name)?.Value;
    }
}