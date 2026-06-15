using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Domain.ReadModels;

namespace FlightDex.Timetable.Application.Enrichment;

public sealed class FlightViewProjector
{
    private readonly IRouteLookup _routeLookup;
    private readonly ILocationLookup _locationLookup;

    public FlightViewProjector(IRouteLookup routeLookup, ILocationLookup locationLookup)
    {
        _routeLookup = routeLookup;
        _locationLookup = locationLookup;
    }

    public async Task<FlightView?> ProjectAsync(Flight flight, string airlineName, CancellationToken cancellationToken = default)
    {
        var route = await _routeLookup.GetRouteAsync(flight.RouteId, cancellationToken);
        if (route is null) return null;

        var from = await _locationLookup.GetLocationAsync(route.FromAirportCode, cancellationToken);
        var to = await _locationLookup.GetLocationAsync(route.ToAirportCode, cancellationToken);
        if (from is null || to is null) return null;

        return new FlightView
        {
            FlightId = flight.Id.Value,
            AirlineId = flight.AirlineId,
            Airline = airlineName,
            RouteId = flight.RouteId,
            FromAirportCode = from.AirportCode,
            FromCity = from.City,
            FromState = from.State,
            FromCountry = from.Country,
            ToAirportCode = to.AirportCode,
            ToCity = to.City,
            ToState = to.State,
            ToCountry = to.Country,
            DepartureUtc = flight.Schedule.DepartureUtc,
            ArrivalUtc = flight.Schedule.ArrivalUtc,
            Status = flight.Status
        };
    }
}
