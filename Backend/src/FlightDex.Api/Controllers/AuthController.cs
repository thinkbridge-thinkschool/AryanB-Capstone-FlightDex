using FlightDex.Api.Contracts;
using FlightDex.Booking.Application;
using FlightDex.Booking.Application.Commands.Login;
using FlightDex.Booking.Application.Commands.RegisterUser;
using FlightDex.Booking.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(ICommandDispatcher dispatcher) : ControllerBase
{
    /// <summary>Registers a new account and returns a token (signed in immediately).</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Problem("Email and password are required.", statusCode: StatusCodes.Status400BadRequest);
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
            return Problem("First and last name are required.", statusCode: StatusCodes.Status400BadRequest);
        if (request.Age <= 0 || request.Age > 130)
            return Problem("Age must be between 1 and 130.", statusCode: StatusCodes.Status400BadRequest);

        var command = new RegisterUserCommand(
            request.Email, request.FirstName, request.LastName, request.Age,
            request.IsGovernmentOfficial, request.IsLawEnforcementOrMilitary, request.Password);

        try
        {
            var result = await dispatcher.DispatchAsync(command, cancellationToken);
            return Ok(result);
        }
        catch (EmailAlreadyInUseException ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
        }
    }

    /// <summary>Authenticates an existing account and returns a fresh token.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await dispatcher.DispatchAsync(
                new LoginCommand(request.Email, request.Password), cancellationToken);
            return Ok(result);
        }
        catch (InvalidCredentialsException ex)
        {
            return Problem(ex.Message, statusCode: StatusCodes.Status401Unauthorized);
        }
    }
}
