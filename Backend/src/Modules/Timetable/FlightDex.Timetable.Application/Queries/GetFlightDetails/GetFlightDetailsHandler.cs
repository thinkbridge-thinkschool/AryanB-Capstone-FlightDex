using FlightDex.Timetable.Application.Contracts;
using FlightDex.Timetable.Application.Queries.GetFlightTimetable;

namespace FlightDex.Timetable.Application.Queries.GetFlightDetails;

/// <summary>Returns the enriched detail view for a single flight from the FlightView store.</summary>
public sealed class GetFlightDetailsHandler
{
    private readonly IFlightViewReadStore _readStore;

    public GetFlightDetailsHandler(IFlightViewReadStore readStore) => _readStore = readStore;

    public Task<FlightDetailsDto?> HandleAsync(
        GetFlightDetailsQuery query,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
