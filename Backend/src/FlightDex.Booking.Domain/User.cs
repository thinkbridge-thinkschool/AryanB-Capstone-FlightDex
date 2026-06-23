namespace FlightDex.Booking.Domain;

/// <summary>
/// A registered account. Authenticates with email + password (only the PBKDF2 hash and
/// its salt are stored, never the password). FirstName / LastName / Age are copied onto
/// each <see cref="Ticket"/> the user books.
/// </summary>
public sealed class User
{
    public int Id { get; private set; }

    public string Email { get; private set; } = string.Empty;
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public int Age { get; private set; }

    public bool IsGovernmentOfficial { get; private set; }
    public bool IsLawEnforcementOrMilitary { get; private set; }

    /// <summary>Base64 PBKDF2 hash of the password.</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>Base64 per-user salt the hash was derived with.</summary>
    public string PasswordSalt { get; private set; } = string.Empty;

    private User() { } // EF Core

    public User(
        string email,
        string firstName,
        string lastName,
        int age,
        bool isGovernmentOfficial,
        bool isLawEnforcementOrMilitary,
        string passwordHash,
        string passwordSalt)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        IsGovernmentOfficial = isGovernmentOfficial;
        IsLawEnforcementOrMilitary = isLawEnforcementOrMilitary;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
    }
}
