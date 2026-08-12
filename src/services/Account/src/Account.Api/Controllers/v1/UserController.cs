using Account.Api.Common;
using Account.Application.Features.Users.Commands.CreateUser;
using Account.SharedKernel.Common.Primitives;
using Account.SharedKernel.Extensions;
using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Account.Api.Controllers.v1;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/user")]
public class UserController : BaseController
{
    public UserController(ISender sender) : base(sender) { }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="command">
    /// The command containing the user registration information, including personal details and authentication credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A response containing the result of the user creation operation.
    /// </returns>
    /// <remarks>
    /// **Example usage:**
    ///
    /// ```http
    /// POST /api/v1/user/register
    /// ```
    ///
    /// **Request:**
    ///
    /// ```json
    /// {
    ///   "firstname": "John",
    ///   "lastname": "Doe",
    ///   "username": "johndoe",
    ///   "email": "john.doe@example.com",
    ///   "password": "Password123!"
    /// }
    /// ```
    /// </remarks>
    /// <response code="201">
    /// The user account was created successfully.
    /// </response>
    /// <response code="400">
    /// The request contains invalid data.
    /// </response>
    /// <response code="409">
    /// An account with the provided email or username already exists.
    /// </response>
    /// <response code="500">
    /// An unexpected internal server error occurred.
    /// </response>
    [HttpPost("register")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
    public async Task<IResult> CreateUser(CreateUserCommand command, CancellationToken cancellationToken = default)
    {
        Result result = await Sender.Send(command, cancellationToken);
        
        return result.Match(
            onSuccess: Results.Created,
            onFailure: Problem);
    }
}