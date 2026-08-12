using Serilog;
using Serilog.Events;

namespace Account.Api.Extensions;

public static class RequestLoggingExtensions
{
    public static WebApplication UseRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            // Custom message template
            options.MessageTemplate = "Handled {RequestPath} in {Elapsed}ms with the status code {StatusCode}";
    
            // Determines the log level based on the response status code or thrown exceptions.
            options.GetLevel = (context, elapsed, exception) =>
            {
                if (exception is not null)
                    return LogEventLevel.Error;

                if (context.Response.StatusCode >= 500)
                    return LogEventLevel.Error;

                if (context.Response.StatusCode >= 400)
                    return LogEventLevel.Warning;

                return LogEventLevel.Information;
            };
    
            // Attach additional properties that can be written to log files.
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RequestPath", httpContext.Request.Path);
                diagnosticContext.Set("RequestQueryString", httpContext.Request.QueryString.Value);
                diagnosticContext.Set("IpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.ToString());
                diagnosticContext.Set("RequestMethod", httpContext.Request.Method);
            };
        });
        
        return app;
    }
}