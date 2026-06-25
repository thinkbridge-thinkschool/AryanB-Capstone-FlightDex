using FlightDex.Flights.Application.Dtos;
using FlightDex.Flights.Domain;

namespace FlightDex.UnitTests.Flights;

public sealed class FlightDtoMappingTests
{
    private static Flight SampleDeparture() => new(
        airport: "BLR",
        direction: FlightDirection.Departure,
        scheduledTime: new TimeOnly(8, 5),
        airlineCode: "AI",
        airline: "Air India",
        counterpartAirport: "Indira Gandhi International Airport",
        counterpartCode: "DEL",
        counterpartCity: "Delhi",
        flightCode: "AI501",
        duration: "2h 45m");

    [Fact]
    public void FlightListItem_FromDomain_maps_every_field_and_formats_time()
    {
        var item = FlightListItem.FromDomain(SampleDeparture());

        Assert.Equal("AI501", item.FlightCode);
        Assert.Equal("08:05", item.Time);          // HH:mm, zero-padded
        Assert.Equal("Air India", item.Airline);
        Assert.Equal("Delhi", item.City);          // counterpart city
        Assert.Equal("DEL", item.Code);            // counterpart code
        Assert.Equal("Indira Gandhi International Airport", item.Airport);
        Assert.Equal(FlightDirection.Departure, item.Direction);
    }

    [Fact]
    public void FlightDetail_FromDomain_maps_every_field_and_formats_time()
    {
        var detail = FlightDetail.FromDomain(SampleDeparture());

        Assert.Equal("AI501", detail.FlightCode);
        Assert.Equal("BLR", detail.Airport);
        Assert.Equal(FlightDirection.Departure, detail.Direction);
        Assert.Equal("08:05", detail.Time);
        Assert.Equal("AI", detail.AirlineCode);
        Assert.Equal("Air India", detail.Airline);
        Assert.Equal("Indira Gandhi International Airport", detail.CounterpartAirport);
        Assert.Equal("DEL", detail.CounterpartCode);
        Assert.Equal("Delhi", detail.CounterpartCity);
        Assert.Equal("2h 45m", detail.Duration);
    }

    [Fact]
    public void FlightListItem_formats_midnight_and_late_times_with_two_digits()
    {
        var midnight = FlightListItem.FromDomain(new Flight(
            "BOM", FlightDirection.Arrival, new TimeOnly(0, 0),
            "6E", "IndiGo", "Pune Airport", "PNQ", "Pune", "6E123", "1h 0m"));

        Assert.Equal("00:00", midnight.Time);
        Assert.Equal(FlightDirection.Arrival, midnight.Direction);
    }
}
