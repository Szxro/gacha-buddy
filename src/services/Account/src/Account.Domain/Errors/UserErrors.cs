using Account.SharedKernel.Common.Primitives;

namespace Account.Domain.Errors;

public class UserErrors
{
    public static Error EmailNotUnique
        => Error.Validation("The provided email is already registered.");

    public static Error UsernameNotUnique
        => Error.Validation("The provided username is already registered.");
}