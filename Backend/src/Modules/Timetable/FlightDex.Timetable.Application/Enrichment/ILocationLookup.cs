namespace FlightDex.Timetable.Application.Enrichment;

public interface ILocationLookup
{
    Task<LocationLookupResult?> GetLocationAsync(string airportCode, CancellationToken cancellationToken = default);
}

public sealed record LocationLookupResult(
    string AirportCode,
    string City,
    string State,
    string Country);
