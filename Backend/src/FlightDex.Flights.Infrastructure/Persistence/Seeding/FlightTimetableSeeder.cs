using Microsoft.Extensions.Logging;

namespace FlightDex.Flights.Infrastructure.Persistence.Seeding;

/// <summary>
/// Loads the daily timetable from the bundled CSV exports into the Flights table on startup.
/// Idempotent: does nothing if the table already has rows.
/// </summary>
public sealed class FlightTimetableSeeder(
    FlightsDbContext dbContext,
    ILogger<FlightTimetableSeeder> logger)
{
    public Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // TODO: skip if already seeded; parse Departures_*/Arrivals_* CSVs and bulk-insert.
        throw new NotImplementedException();
    }
}
