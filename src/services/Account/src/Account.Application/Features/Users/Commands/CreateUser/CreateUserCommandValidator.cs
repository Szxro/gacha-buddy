using FluentValidation;

namespace Account.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotNull().WithMessage("The email address can't be null.")
            .NotEmpty().WithMessage("The email address can't be empty.")
            .EmailAddress(). WithMessage("The email address is not valid.");

        RuleFor(command => command.PassWord)
            .NotNull().WithMessage("The password can't be null.")
            .NotEmpty().WithMessage("The password can't be empty.")
            .MinimumLength(8).WithMessage("The password must be at least 8 characters long.")
            .Matches(@"[A-Z]").WithMessage("The password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("The password must contain at least one lowercase letter.")
            .Matches(@"[0-9]").WithMessage("The password must contain at least one number.")
            .Matches(@"[\W_]").WithMessage("The password must contain at least one special character.");

        RuleFor(command => command.UserName)
            .NotNull().WithMessage("The username can't be null")
            .NotEmpty().WithMessage("The username can't be empty")
            .MinimumLength(3).WithMessage("The username must contain at least three characters.")
            .MaximumLength(20).WithMessage("The username must not exceed 20 characters.")
            .Matches("^[a-zA-Z0-9_]+$")
            .WithMessage("The username can only contain letters, numbers, and underscores.");;

        RuleFor(command => command.FirstName)
            .NotNull().WithMessage("The firstname can't be null.")
            .NotEmpty().WithMessage("The firstname can't be empty.");

        RuleFor(command => command.LastName)
            .NotNull().WithMessage("The lastname can't be null.")
            .NotEmpty().WithMessage("The lastname can't be empty.");
    }
}