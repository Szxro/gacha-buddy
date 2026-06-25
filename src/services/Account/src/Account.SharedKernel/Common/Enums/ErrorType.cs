namespace Account.SharedKernel.Common.Enums;

public enum ErrorType
{
    // Common
    None = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    
    //Security
    Unauthorized = 4,
    Forbidden = 5,
    
    // Operation (500)
    Failure = 6
}