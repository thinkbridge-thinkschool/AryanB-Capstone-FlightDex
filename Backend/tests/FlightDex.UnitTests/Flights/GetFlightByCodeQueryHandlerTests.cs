using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Queries.GetFlightByCode;
using FlightDex.Flights.Domain;
using NSubstitute;

namespace FlightDex.UnitTests.Flights;

public sealed class GetFlightByCodeQueryHandlerTests
{
    private readonly IFlightRepository _repository = Substitute.For<IFlightRepository>();

    [Fact]
    public async Task Handler_returns_empty_when_no_rows_match()
    {
        _repository.GetByFlightCodeAsync("ZZ999", Arg.Any<CancellationToken>())
            .Returns(Array.Empty<Flight>());

        var handler = new GetFlightByCodeQueryHandler(_repository);
        var result = await handler.HandleAsync(new GetFlightByCodeQuery("ZZ999"));

        Assert.Empty(result);
    }

    [Fact]
    public async Task Handler_maps_each_matching_row_to_a_detail()
    {
        Flight Leg(FlightDirection d) => new(
            "BLR", d, new TimeOnly(8, 0), "AI", "Air India",
            "Indira Gandhi International Airport", "DEL", "Delhi", "AI501", "2h 45m");

        _repository.GetByFlightCodeAsync("AI501", Arg.Any<CancellationToken>())
            .Returns([Leg(FlightDirection.Departure), Leg(FlightDirection.Arrival)]);

        var handler = new GetFlightByCodeQueryHandler(_repository);
        var result = await handler.HandleAsync(new GetFlightByCodeQuery("AI501"));

        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal("AI501", d.FlightCode));
        Assert.Contains(result, d => d.Direction == FlightDirection.Departure);
        Assert.Contains(result, d => d.Direction == FlightDirection.Arrival);
    }
}
