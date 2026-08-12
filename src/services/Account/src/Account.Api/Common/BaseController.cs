using Account.Application.Common.Errors;
using Account.SharedKernel.Common.Enums;
using Account.SharedKernel.Common.Primitives;
using Account.SharedKernel.Common.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Common;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected readonly ISender Sender;

    public BaseController(ISender sender)
    {
        Sender = sender;
    }
    
    protected IResult Success<TValue>(Result<TValue> result)
    {
        if (result.IsFailure)
        {
            throw new InvalidOperationException("Result indicates failure, but Success method was called.");
        }

        SuccessResponse<TValue> response = new SuccessResponse<TValue>
        {
            Data = result.Value
        };

        return Results.Ok(response);
    }
    
    protected IResult Problem(Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException("Result indicates success, but Problem method was called.");
        }

        (string title, string detail, string type, int statusCode) = GetErrorDetails(result.Error);

        return Results.Problem(
            title: title,
            detail: detail,
            type: type,
            statusCode: statusCode,
            instance:HttpContext.Request.Path,
            extensions: GetErrorsFromResult(result));
    }
    
    private (string title, string detail, string type, int statusCode) GetErrorDetails(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.1", StatusCodes.Status400BadRequest),
            ErrorType.NotFound => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.4", StatusCodes.Status404NotFound),
            ErrorType.Conflict => (error.ErrorCode, error.Description, "https://tools.ietf.org/html/rfc7231#section-6.5.8", StatusCodes.Status409Conflict),
            ErrorType.Unauthorized => (error.ErrorCode, error.Description, "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1", StatusCodes.Status401Unauthorized),
            ErrorType.Forbidden => (error.ErrorCode, error.Description, "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3", StatusCodes.Status403Forbidden),
            _ => ("Server Failure.", "An unexpected error occurred.", "https://tools.ietf.org/html/rfc7231#section-6.6.1", StatusCodes.Status500InternalServerError)
        };
    }
    private Dictionary<string, object?>? GetErrorsFromResult(Result result)
    {
        if (result.Error is not ValidationError validationError)
        {
            return null;
        }

        return new Dictionary<string, object?>
        {
            { "errors", validationError.Errors }
        };
    }
}