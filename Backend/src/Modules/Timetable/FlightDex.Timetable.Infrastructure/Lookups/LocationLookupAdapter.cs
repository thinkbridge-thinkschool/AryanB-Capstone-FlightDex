using FlightDex.Locations.Application.Queries.GetLocationByCode;
using FlightDex.Timetable.Application.Enrichment;

namespace FlightDex.Timetable.Infrastructure.Lookups;

public sealed class LocationLookupAdapter : ILocationLookup
{
    private readonly GetLocationByCodeHandler _getLocationByCode;

    public LocationLookupAdapter(GetLocationByCodeHandler getLocationByCode)
        => _getLocationByCode = getLocationByCode;

    public async Task<LocationLookupResult?> GetLocationAsync(string airportCode, CancellationToken cancellationToken = default)
    {
        var dto = await _getLocationByCode.HandleAsync(new GetLocationByCodeQuery(airportCode), cancellationToken);
        if (dto is null) return null;
        return new LocationLookupResult(dto.AirportCode, dto.City, dto.State, dto.Country);
    }
}
