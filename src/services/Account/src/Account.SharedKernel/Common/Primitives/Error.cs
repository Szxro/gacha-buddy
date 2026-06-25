using Account.SharedKernel.Common.Enums;

namespace Account.SharedKernel.Common.Primitives;

public class Error
{
    public string ErrorCode { get; } 
    
    public string Description { get; } 

    public ErrorType Type { get; }
    
    protected Error(
        string errorCode,
        string description,
        ErrorType errorType)
    {
        ErrorCode = errorCode;
        Description = description;
        Type = errorType;
    }
    
    public static readonly Error None = new Error(string.Empty, string.Empty, ErrorType.None);
    
    public static Error Validation(string description) => new Error("Error.Validation", description, ErrorType.Validation);

    public static Error NotFound(string description) => new Error("Error.NotFound", description, ErrorType.NotFound);
    
    public static Error Conflict(string description) => new Error("Error.Conflict", description, ErrorType.Conflict);
    
    public static Error Unauthorized(string description) => new Error("Error.Unauthorized", description, ErrorType.Unauthorized);
    
    public static Error Forbidden(string description) => new Error("Error.Forbidden", description, ErrorType.Forbidden);
    
    public static Error Failure(string description) => new Error("Error.Failure", description, ErrorType.Failure);
}