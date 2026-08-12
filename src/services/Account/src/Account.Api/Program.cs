using Account.Api.Extensions;
using Account.Api.Middleware;
using Account.Application;
using Account.Infrastructure;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    // Load environment variables
    DotNetEnv.Env.Load();
    
    // Adding Serilog
    builder.Host.UseSerilog((host, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(host.Configuration));
    
    // Add environment variables to configuration
    builder.Configuration.AddEnvironmentVariables();
    
    // Add services to the container.
    builder.Services
        .AddApplicationLayer()
        .AddInfrastructureLayer(builder.Environment)
        .AddPresentationLayer(builder.Environment);
}

WebApplication app = builder.Build();
{
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerDocumentation();
    }

    app.UseRequestLogging();
    
    app.UseHttpsRedirection();
    
    app.UseExceptionHandler();

    app.UseCors("default");
    
    app.UseAuthentication();

    app.UseAuthorization();

    app.UseRateLimiter();

    app.MapControllers();
    
    app.UseMiddleware<FallBackRouteMiddleware>();

    app.Run();
}