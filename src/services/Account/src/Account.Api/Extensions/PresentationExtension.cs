using System.IdentityModel.Tokens.Jwt;
using System.Threading.RateLimiting;
using Account.Api.ExceptionHandlers;
using Account.Api.Middleware;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Account.Api.Extensions;

public static class PresentationExtension
{
    public static IServiceCollection AddPresentationLayer(this IServiceCollection services, IHostEnvironment env)
    {
        // Authorization and Authorization
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddAuthorization();
        
        // Core Services
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddProblemDetails();
        
        // Rate Limiting
        services.AddRateLimiter(options =>
        {
            // 100 request per minute (Globally by user identity or ip address) 
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                string? userId = httpContext.User.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    return RateLimitPartition.GetFixedWindowLimiter<string>(
                        userId,
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            AutoReplenishment = true
                        });
                }
                
                string ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                
                return RateLimitPartition.GetFixedWindowLimiter<string>(
                    ip,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });
            
            // Custom rejection handling logic (response)
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.Headers["X-RateLimit-Limit"] = "100";
                context.HttpContext.Response.Headers["X-RateLimit-Remaining"] ="0";
                context.HttpContext.Response.Headers["X-RateLimit-Reset"]= DateTimeOffset.UtcNow.AddMinutes(1).ToUnixTimeSeconds().ToString(); 
                context.HttpContext.Response.Headers.RetryAfter = "60";
                context.HttpContext.Response.ContentType = "application/problem+json";

                ProblemDetails problemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status429TooManyRequests,
                    Title = "Rate limit exceeded",
                    Detail = "You have exceeded the allowed number of requests. Please wait before trying again.",
                    Type = "https://datatracker.ietf.org/doc/html/rfc6585#section-4",
                    Instance = context.HttpContext.Request.Path
                };

                await context.HttpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            };
        });
        
        // API versioning
        services
            .AddApiVersioning(options =>
            {
                // Default api version (1.0)
                options.DefaultApiVersion = new ApiVersion(1);
                // Version strategy
                options.ApiVersionReader = ApiVersionReader.Combine( //(can combine the strategies)
                    new UrlSegmentApiVersionReader(), // v1/path
                    new HeaderApiVersionReader("X-API-Version")); // X-API-VERSION: 1.0
                // If the user does not specify a version, you can let the API use the default version
                options.AssumeDefaultVersionWhenUnspecified = true;
                // Reports the supported API versions in the api-supported-versions response header.
                options.ReportApiVersions = true;
            })
            .AddMvc() // Needed for controllers 
            .AddApiExplorer(options =>
            {
                // GroupNameFormat specifies the format of the API version.
                options.GroupNameFormat = "'v'VVV";
                // Tells the API Explorer to replace the {version:apiVersion} placeholder with the actual version number.
                options.SubstituteApiVersionInUrl = true;
            });
        
        // Swagger
        services.AddSwaggerDocumentation(env);
        
        // Exception Handlers
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // Middlewares (Register factory-activated middleware if it has injected dependencies.)
        services.AddTransient<FallBackRouteMiddleware>(); 
        
        // Cors
        services.AddCors(options =>
        {
            options.AddPolicy("default", policy =>
            {
                policy.SetIsOriginAllowed(origin => true);
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();                                
            });
        });
        
        return services;
    }
}