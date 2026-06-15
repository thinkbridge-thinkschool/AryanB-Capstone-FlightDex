using FlightDex.Timetable.Application.Contracts;
using FlightDex.Timetable.Application.Queries.GetFlightTimetable;

namespace FlightDex.Timetable.Application.Queries.GetFlightDetails;

public sealed class GetFlightDetailsHandler
{
    private readonly IFlightViewReadStore _readStore;

    public GetFlightDetailsHandler(IFlightViewReadStore readStore) => _readStore = readStore;

    public async Task<FlightDetailsDto?> HandleAsync(
        GetFlightDetailsQuery query,
        CancellationToken cancellationToken = default)
    {
        var v = await _readStore.GetByFlightIdAsync(query.FlightId, cancellationToken);
        if (v is null) return null;

        return new FlightDetailsDto(
            v.FlightId,
            v.Airline,
            v.FromAirportCode,
            v.FromCity,
            v.FromState,
            v.FromCountry,
            v.ToAirportCode,
            v.ToCity,
            v.ToState,
            v.ToCountry,
            v.DepartureUtc,
            v.ArrivalUtc,
            v.Status);
    }
}
