namespace FlightDex.Api.Contracts;

/// <summary>Registration payload — all profile fields plus the chosen password.</summary>
public sealed record RegisterRequest(
    string Email,
    string FirstName,
    string LastName,
    int Age,
    bool IsGovernmentOfficial,
    bool IsLawEnforcementOrMilitary,
    string Password);

/// <summary>Login payload.</summary>
public sealed record LoginRequest(string Email, string Password);
