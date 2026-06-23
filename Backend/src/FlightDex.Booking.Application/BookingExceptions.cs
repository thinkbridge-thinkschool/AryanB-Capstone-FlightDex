namespace FlightDex.Booking.Application;

/// <summary>Thrown when a registration uses an email that already exists.</summary>
public sealed class EmailAlreadyInUseException(string email)
    : Exception($"The email '{email}' is already in use.");

/// <summary>Thrown when login credentials don't match.</summary>
public sealed class InvalidCredentialsException()
    : Exception("Invalid email or password.");
