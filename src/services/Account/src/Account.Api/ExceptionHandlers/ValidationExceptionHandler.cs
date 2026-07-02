using Account.SharedKernel.Common.Primitives;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.ExceptionHandlers;

public class ValidationExceptionHandler : IExceptionHandler
{
    private readonly ILogger<ValidationExceptionHandler> _logger;

    public ValidationExceptionHandler(ILogger<ValidationExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    { 
        if (exception is not ValidationException validationException) return false;
        
        Dictionary<string, object?>? failures = GetErrorsFromException(validationException);
        
        ProblemDetails problemDetails = new ProblemDetails
        {
            Title = "Validation.Error",
            Detail = "One or more validation errors occurred.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Status = StatusCodes.Status400BadRequest
        };

        if (failures is not null)
        {
            problemDetails.Extensions = failures;
        }
        
        httpContext.Response.StatusCode = (int)problemDetails.Status;

        httpContext.Response.ContentType = "application/problem+json";
        
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static Dictionary<string, object?>? GetErrorsFromException(ValidationException exception)
    {
        List<Error> failures = exception.Errors.Select(x => Error.Validation(x.ErrorMessage)).ToList();

        return new Dictionary<string, object?>()
        {
            {"errors", failures }
        };
    }
}