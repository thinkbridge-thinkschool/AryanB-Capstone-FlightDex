namespace FlightDex.Booking.Application;

/// <summary>Registration was attempted with an email that already has an account.</summary>
public sealed class EmailAlreadyInUseException(string email)
    : Exception($"An account already exists for '{email}'.");

/// <summary>Login failed: unknown email or wrong password.</summary>
public sealed class InvalidCredentialsException()
    : Exception("Invalid email or password.");
