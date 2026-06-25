using FlightDex.Booking.Infrastructure.Security;

namespace FlightDex.UnitTests.Booking;

public sealed class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_then_Verify_succeeds_for_the_same_password()
    {
        var (hash, salt) = _hasher.Hash("correct horse battery staple");
        Assert.True(_hasher.Verify("correct horse battery staple", hash, salt));
    }

    [Fact]
    public void Verify_fails_for_a_wrong_password()
    {
        var (hash, salt) = _hasher.Hash("s3cret");
        Assert.False(_hasher.Verify("not-the-password", hash, salt));
    }

    [Fact]
    public void Each_hash_uses_a_fresh_salt_so_the_same_password_hashes_differently()
    {
        var a = _hasher.Hash("samePassword");
        var b = _hasher.Hash("samePassword");

        Assert.NotEqual(a.Salt, b.Salt);
        Assert.NotEqual(a.Hash, b.Hash);
    }

    [Fact]
    public void Verify_returns_false_for_a_malformed_base64_hash_rather_than_throwing()
    {
        Assert.False(_hasher.Verify("any", "not-base64!!", "also-not-base64!!"));
    }
}
