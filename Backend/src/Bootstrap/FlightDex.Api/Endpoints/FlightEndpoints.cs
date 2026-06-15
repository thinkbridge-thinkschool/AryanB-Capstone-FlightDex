namespace FlightDex.Api.Endpoints;

/// <summary>Maps the dashboard's flight routes.</summary>
public static class FlightEndpoints
{
    /// <summary>
    /// Maps:
    ///   GET /flight              -> paginated, filtered/sorted timetable
    ///   GET /flight/{flightId}   -> enriched single-flight detail
    /// </summary>
    public static IEndpointRouteBuilder MapFlightEndpoints(this IEndpointRouteBuilder app)
        => throw new NotImplementedException();
}
