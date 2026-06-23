namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Hashes and verifies passwords. The implementation chooses the algorithm.</summary>
public interface IPasswordHasher
{
    /// <summary>Derives a fresh salt and returns the base64 hash + salt to store.</summary>
    (string Hash, string Salt) Hash(string password);

    /// <summary>Constant-time check of a password against a stored hash + salt.</summary>
    bool Verify(string password, string hash, string salt);
}
