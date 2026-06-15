using FlightDex.Locations.Application.Queries.GetLocationByCode;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Infrastructure.Lookups;

/// <summary><see cref="ILocationLookup"/> implementation calling the Locations context's query API.</summary>
public sealed class LocationLookupAdapter : ILocationLookup
{
    private readonly GetLocationByCodeHandler _getLocationByCode;

    public LocationLookupAdapter(GetLocationByCodeHandler getLocationByCode) => _getLocationByCode = getLocationByCode;

    public Task<LocationLookupResult?> GetLocationAsync(string airportCode, CancellationToken cancellationToken = default)
        => throw new NotImplementedException();
}
