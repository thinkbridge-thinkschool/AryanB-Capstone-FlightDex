using FlightDex.Api.Contracts;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

/// <summary>Slice 2 — registration and login. Both return a JWT.</summary>
[ApiController]
[Route("auth")]
public sealed class AuthController(ICommandDispatcher dispatcher) : ControllerBase
{
    /// <summary>Registers a new account and returns a token (signed in immediately).</summary>
    [HttpPost("register")]
    public Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        // TODO: validate, dispatch RegisterUserCommand, map EmailAlreadyInUse -> 409.
        throw new NotImplementedException();
    }

    /// <summary>Authenticates an existing account and returns a fresh token.</summary>
    [HttpPost("login")]
    public Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        // TODO: dispatch LoginCommand, map InvalidCredentials -> 401.
        throw new NotImplementedException();
    }
}
