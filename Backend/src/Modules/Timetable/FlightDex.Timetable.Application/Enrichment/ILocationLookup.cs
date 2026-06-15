namespace FlightDex.Timetable.Application.Enrichment;

/// <summary>Port to resolve an airport code into name + city/state/country (Locations context).</summary>
public interface ILocationLookup
{
    Task<LocationLookupResult?> GetLocationAsync(string airportCode, CancellationToken cancellationToken = default);
}

/// <summary>Location detail as seen by the Timetable enrichment pipeline.</summary>
public sealed record LocationLookupResult(
    string AirportCode,
    string AirportName,
    string City,
    string State,
    string Country);
