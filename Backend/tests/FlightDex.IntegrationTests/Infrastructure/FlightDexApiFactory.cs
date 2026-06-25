using FlightDex.Flights.Domain;
using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.IntegrationTests.Infrastructure;

/// <summary>
/// Boots the real API (Program) in-process against throwaway SQLite databases, then
/// replaces the timetable with a small, known dataset so assertions are deterministic
/// regardless of whatever the CSV seeder loaded at startup.
/// </summary>
public sealed class FlightDexApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly string _flightsDb = Path.Combine(Path.GetTempPath(), $"flightdex-it-flights-{Guid.NewGuid():N}.db");
    private readonly string _bookingDb = Path.Combine(Path.GetTempPath(), $"flightdex-it-booking-{Guid.NewGuid():N}.db");

    public FlightDexApiFactory()
    {
        // Read by the default environment-variables configuration provider before services
        // are registered, so AddFlightsInfrastructure/AddBookingInfrastructure pick these up.
        Environment.SetEnvironmentVariable("ConnectionStrings__FlightDex", $"Data Source={_flightsDb}");
        Environment.SetEnvironmentVariable("ConnectionStrings__Booking", $"Data Source={_bookingDb}");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.UseEnvironment("Testing");

    public async Task InitializeAsync()
    {
        // Force the host to build (runs migrations + startup seed), then overwrite with our set.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FlightsDbContext>();

        await db.Locations.ExecuteDeleteAsync();
        await db.Flights.ExecuteDeleteAsync();

        db.Flights.AddRange(KnownTimetable());
        db.Locations.AddRange(
            new Location("DEL", "Indira Gandhi International Airport", "Delhi"),
            new Location("BOM", "Chhatrapati Shivaji Maharaj International Airport", "Mumbai"),
            new Location("BLR", "Kempegowda International Airport", "Bengaluru"));
        await db.SaveChangesAsync();
    }

    /// <summary>5 BLR departures (4 to DEL, 1 to BOM) and 2 BLR arrivals, plus 1 BOM departure.</summary>
    public static IReadOnlyList<Flight> KnownTimetable() =>
    [
        Dep("BLR", new(6, 0),  "AI501", "DEL", "Delhi"),
        Dep("BLR", new(8, 5),  "AI502", "DEL", "Delhi"),
        Dep("BLR", new(9, 30), "6E101", "BOM", "Mumbai"),
        Dep("BLR", new(14, 0), "UK810", "DEL", "Delhi"),
        Dep("BLR", new(22, 15),"AI999", "DEL", "Delhi"),
        Arr("BLR", new(7, 0),  "AI601", "DEL", "Delhi"),
        Arr("BLR", new(10, 0), "6E201", "BOM", "Mumbai"),
        Dep("BOM", new(8, 0),  "AI700", "BLR", "Bengaluru"),
    ];

    private static Flight Dep(string airport, TimeOnly time, string code, string toCode, string toCity) =>
        new(airport, FlightDirection.Departure, time, code[..2], Airline(code), $"{toCity} Airport", toCode, toCity, code, "2h 0m");

    private static Flight Arr(string airport, TimeOnly time, string code, string fromCode, string fromCity) =>
        new(airport, FlightDirection.Arrival, time, code[..2], Airline(code), $"{fromCity} Airport", fromCode, fromCity, code, "2h 0m");

    private static string Airline(string code) => code[..2] switch
    {
        "AI" => "Air India",
        "6E" => "IndiGo",
        "UK" => "Vistara",
        _ => "Test Air",
    };

    public new async Task DisposeAsync()
    {
        await base.DisposeAsync();
        TryDelete(_flightsDb);
        TryDelete(_bookingDb);
    }

    private static void TryDelete(string path)
    {
        foreach (var p in new[] { path, path + "-shm", path + "-wal" })
        {
            try { if (File.Exists(p)) File.Delete(p); } catch { /* best effort */ }
        }
    }
}

[CollectionDefinition("api")]
public sealed class ApiCollection : ICollectionFixture<FlightDexApiFactory>;
