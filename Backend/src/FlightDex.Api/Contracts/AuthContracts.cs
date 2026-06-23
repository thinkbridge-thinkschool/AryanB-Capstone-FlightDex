namespace FlightDex.Api.Contracts;

/// <summary>Request body for POST /auth/register.</summary>
public sealed record RegisterRequest(
    string Email,
    string FirstName,
    string LastName,
    int Age,
    bool IsGovernmentOfficial,
    bool IsLawEnforcementOrMilitary,
    string Password);

/// <summary>Request body for POST /auth/login.</summary>
public sealed record LoginRequest(string Email, string Password);
