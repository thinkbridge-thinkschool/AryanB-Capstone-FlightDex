using FlightDex.Flights.Application;

namespace FlightDex.UnitTests.Flights;

public sealed class ServedAirportsTests
{
    [Theory]
    [InlineData("BLR")]
    [InlineData("BOM")]
    [InlineData("PNQ")]
    [InlineData("LON")]
    [InlineData("DBX")]
    public void IsServed_is_true_for_every_served_airport(string code)
        => Assert.True(ServedAirports.IsServed(code));

    [Theory]
    [InlineData("blr")]
    [InlineData("Bom")]
    [InlineData("pNq")]
    public void IsServed_is_case_insensitive(string code)
        => Assert.True(ServedAirports.IsServed(code));

    [Theory]
    [InlineData("DEL")]   // a real airport, but not one we serve
    [InlineData("XXX")]
    [InlineData("")]
    public void IsServed_is_false_for_unserved_codes(string code)
        => Assert.False(ServedAirports.IsServed(code));

    [Fact]
    public void IsServed_is_false_for_null()
        => Assert.False(ServedAirports.IsServed(null));

    [Fact]
    public void All_contains_exactly_the_five_served_airports()
        => Assert.Equal(5, ServedAirports.All.Count);
}
