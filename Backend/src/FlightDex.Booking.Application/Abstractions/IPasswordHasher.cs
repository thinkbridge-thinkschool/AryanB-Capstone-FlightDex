namespace FlightDex.Booking.Application.Abstractions;

/// <summary>Hashes and verifies passwords (PBKDF2).</summary>
public interface IPasswordHasher
{
    (string Hash, string Salt) Hash(string password);
    bool Verify(string password, string hash, string salt);
}
