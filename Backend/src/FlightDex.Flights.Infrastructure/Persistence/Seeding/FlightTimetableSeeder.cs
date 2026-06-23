using System.Globalization;
using FlightDex.Flights.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FlightDex.Flights.Infrastructure.Persistence.Seeding;

/// <summary>
/// Loads the daily timetable from the bundled CSV exports into the Flights table on
/// startup. Idempotent: does nothing if the table already has rows.
///
/// Both departure and arrival files share the same column order, so rows are parsed by
/// position rather than by header (headers vary in casing across files):
///   0 Time | 1 AirlineCode | 2 Airline | 3 CounterpartAirport
///   4 CounterpartCode | 5 CounterpartCity | 6 FlightCode | 7 Duration
/// Direction and airport come from the file name, e.g. "Departures_BLR.csv".
/// </summary>
public sealed class FlightTimetableSeeder(
    FlightsDbContext dbContext,
    ILogger<FlightTimetableSeeder> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (await dbContext.Flights.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Flights table already seeded; skipping.");
            return;
        }

        var seedDir = Path.Combine(AppContext.BaseDirectory, "Persistence", "SeedData");
        if (!Directory.Exists(seedDir))
        {
            logger.LogWarning("Seed data directory not found at {Dir}; nothing seeded.", seedDir);
            return;
        }

        var flights = new List<Flight>();
        foreach (var path in Directory.EnumerateFiles(seedDir, "*.csv"))
            flights.AddRange(ParseFile(path));

        if (flights.Count == 0)
        {
            logger.LogWarning("No flight rows parsed from {Dir}.", seedDir);
            return;
        }

        await dbContext.Flights.AddRangeAsync(flights, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} flight rows.", flights.Count);
    }

    private IEnumerable<Flight> ParseFile(string path)
    {
        var fileName = Path.GetFileNameWithoutExtension(path); // e.g. Departures_BLR
        var parts = fileName.Split('_', 2);
        if (parts.Length != 2)
        {
            logger.LogWarning("Skipping unrecognised seed file {File}.", fileName);
            yield break;
        }

        var direction = parts[0].StartsWith("Arr", StringComparison.OrdinalIgnoreCase)
            ? FlightDirection.Arrival
            : FlightDirection.Departure;
        var airport = parts[1].ToUpperInvariant();

        var lineNumber = 0;
        foreach (var line in File.ReadLines(path))
        {
            lineNumber++;
            if (lineNumber == 1 || string.IsNullOrWhiteSpace(line)) continue; // header / blank

            var f = CsvLineParser.Parse(line);
            if (f.Count < 8)
            {
                logger.LogWarning("Skipping malformed line {Line} in {File}.", lineNumber, fileName);
                continue;
            }

            if (!TryParseTime(f[0], out var time))
            {
                logger.LogWarning("Skipping line {Line} in {File}: bad time '{Time}'.", lineNumber, fileName, f[0]);
                continue;
            }

            yield return new Flight(
                airport: airport,
                direction: direction,
                scheduledTime: time,
                airlineCode: f[1].Trim(),
                airline: f[2].Trim(),
                counterpartAirport: f[3].Trim(),
                counterpartCode: f[4].Trim(),
                counterpartCity: f[5].Trim(),
                flightCode: f[6].Trim(),
                duration: f[7].Trim());
        }
    }

    private static bool TryParseTime(string raw, out TimeOnly time) =>
        TimeOnly.TryParseExact(raw.Trim(), "HH\\:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
}
