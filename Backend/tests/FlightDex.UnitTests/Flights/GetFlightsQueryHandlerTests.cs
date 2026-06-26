using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.Flights.Application.Queries.GetFlights;
using FlightDex.Flights.Domain;
using FlightDex.SharedKernel.Paging;
using NSubstitute;

namespace FlightDex.UnitTests.Flights;

public sealed class GetFlightsQueryHandlerTests
{
    private readonly IFlightRepository _repository = Substitute.For<IFlightRepository>();

    private static Flight Flight(string code, int hour) => new(
        "BLR", FlightDirection.Departure, new TimeOnly(hour, 0),
        "AI", "Air India", "Indira Gandhi International Airport", "DEL", "Delhi", code, "2h 45m");

    [Fact]
    public async Task Handler_passes_the_spec_through_to_the_repository_unchanged()
    {
        var spec = new FlightQuerySpec(
            FlightDirection.Arrival, "BOM", "Delhi", null, new TimeOnly(8, 0), new TimeOnly(12, 0), 2, 15);
        _repository.GetPagedAsync(spec, Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Flight>([], 2, 15, 0));

        var handler = new GetFlightsQueryHandler(_repository);
        await handler.HandleAsync(new GetFlightsQuery(spec));

        await _repository.Received(1).GetPagedAsync(spec, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handler_maps_domain_flights_to_list_items_and_preserves_paging_metadata()
    {
        var spec = new FlightQuerySpec(FlightDirection.Departure, "BLR", null, null, null, null, 1, 30);
        _repository.GetPagedAsync(spec, Arg.Any<CancellationToken>())
            .Returns(new PagedResult<Flight>([Flight("AI501", 8), Flight("AI502", 9)], 1, 30, 57));

        var handler = new GetFlightsQueryHandler(_repository);
        var result = await handler.HandleAsync(new GetFlightsQuery(spec));

        Assert.IsType<PagedResult<FlightListItem>>(result);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(["AI501", "AI502"], result.Items.Select(i => i.FlightCode));
        Assert.Equal(1, result.Page);
        Assert.Equal(30, result.PageSize);
        Assert.Equal(57, result.TotalCount);
        Assert.Equal(2, result.TotalPages);   // ceil(57/30)
    }
}
