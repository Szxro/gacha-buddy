using Account.Application.Common.Abstractions;
using Account.Application.Contracts;
using Account.Domain.Entities;
using Account.Domain.Errors;
using Account.Domain.Events;
using Account.SharedKernel.Common.Attributes;
using Account.SharedKernel.Common.Primitives;

namespace Account.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommand : ICommand ,IAuditableRequest
{
    /// <summary>
    /// The first name of the user.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// The last name of the user.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// The username used to authenticate the user.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// The email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The password used to authenticate the user.
    /// </summary>
    public string PassWord { get; set; } = string.Empty;
    
    [SwaggerExclude]
    public string? ResourceName  => "User";

    [SwaggerExclude]
    public string? ResourceId => null;
    
    public object? GetAuditData()
    {
        // in this case is not a good idea to save passwords in the audit table
        return new
        {
            FirstName,
            LastName,
            UserName,
            Email,
        };
    }
}

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IHashService _hashService;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        IHashService hashService)
    {
        _userRepository = userRepository;
        _hashService = hashService;
    }
    
    public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Result.Failure(UserErrors.EmailNotUnique);
        }
        
        if (await _userRepository.AnyAsync(u => u.Username == request.UserName, cancellationToken))
        {
            return Result.Failure(UserErrors.UsernameNotUnique);
        }

        User newUser = new User
        {
            Firstname = request.FirstName,
            Lastname = request.LastName,
            Email = request.Email,
            Username = request.UserName,
        };
        
        (string hash,byte[] salt) = _hashService.GetHashAndSalt(request.PassWord);
        
        newUser.Credentials.Add(new Credentials
        {
            HashValue =  hash,
            SaltValue = Convert.ToHexString(salt),
            IsActive = true
        });
        
        // TODO: GENERATE EMAIL CODE AND WITH AN DOMAIN EVENT SEND IT 
        
        newUser.AddEvent(new WelcomeEvent{Username =  request.UserName});
        
        _userRepository.Add(newUser);
        
        return Result.Success();
    }
}
    
    