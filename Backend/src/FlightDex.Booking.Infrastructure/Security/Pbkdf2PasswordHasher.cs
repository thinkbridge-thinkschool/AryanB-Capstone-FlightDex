using FlightDex.Booking.Application.Abstractions;

namespace FlightDex.Booking.Infrastructure.Security;

/// <summary>PBKDF2 password hashing with a per-user random salt.</summary>
internal sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    public (string Hash, string Salt) Hash(string password)
    {
        // TODO: generate a salt, derive the PBKDF2 hash, return both Base64-encoded.
        throw new NotImplementedException();
    }

    public bool Verify(string password, string hash, string salt)
    {
        // TODO: re-derive with the stored salt and compare in constant time.
        throw new NotImplementedException();
    }
}
