using FlightDex.Locations.Domain;
using FlightDex.Locations.Infrastructure.Persistence;
using FlightDex.Routing.Infrastructure.Persistence;
using FlightDex.Timetable.Domain.Airline;
using FlightDex.Timetable.Domain.Flight;
using FlightDex.Timetable.Domain.ReadModels;
using FlightDex.Timetable.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using RoutingDomain = FlightDex.Routing.Domain;

namespace FlightDex.Api;

public static class DatabaseSeeder
{
    private static readonly Guid A1 = new("10000000-0000-0000-0000-000000000001");
    private static readonly Guid A2 = new("10000000-0000-0000-0000-000000000002");
    private static readonly Guid A3 = new("10000000-0000-0000-0000-000000000003");
    private static readonly Guid A4 = new("10000000-0000-0000-0000-000000000004");
    private static readonly Guid A5 = new("10000000-0000-0000-0000-000000000005");

    private static readonly Guid R1 = new("20000000-0000-0000-0000-000000000001");
    private static readonly Guid R2 = new("20000000-0000-0000-0000-000000000002");
    private static readonly Guid R3 = new("20000000-0000-0000-0000-000000000003");
    private static readonly Guid R4 = new("20000000-0000-0000-0000-000000000004");
    private static readonly Guid R5 = new("20000000-0000-0000-0000-000000000005");

    private static readonly Guid F1 = new("30000000-0000-0000-0000-000000000001");
    private static readonly Guid F2 = new("30000000-0000-0000-0000-000000000002");
    private static readonly Guid F3 = new("30000000-0000-0000-0000-000000000003");
    private static readonly Guid F4 = new("30000000-0000-0000-0000-000000000004");
    private static readonly Guid F5 = new("30000000-0000-0000-0000-000000000005");

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        await EnsureCreatedAsync(sp);

        var timetableCtx = sp.GetRequiredService<TimetableDbContext>();
        if (await timetableCtx.Airlines.AnyAsync()) return;

        await SeedLocationsAsync(sp);
        await SeedRoutesAsync(sp);
        await SeedAirlinesAndFlightsAsync(sp);
    }

    private static async Task EnsureCreatedAsync(IServiceProvider sp)
    {
        // First context creates the database itself; subsequent contexts only add their tables.
        await sp.GetRequiredService<LocationsDbContext>().Database.EnsureCreatedAsync();
        await EnsureTablesAsync(sp.GetRequiredService<RoutingDbContext>());
        await EnsureTablesAsync(sp.GetRequiredService<TimetableDbContext>());
    }

    private static async Task EnsureTablesAsync(DbContext ctx)
    {
        var creator = ctx.Database.GetService<IRelationalDatabaseCreator>();
        try
        {
            await creator.CreateTablesAsync();
        }
        catch (Microsoft.Data.SqlClient.SqlException)
        {
            // Tables already exist — safe to ignore.
        }
    }

    private static async Task SeedLocationsAsync(IServiceProvider sp)
    {
        var ctx = sp.GetRequiredService<LocationsDbContext>();
        ctx.Locations.AddRange(
            Location.Create(AirportCode.Create("ORD"), Address.Create("Chicago",     "Illinois",    "United States")),
            Location.Create(AirportCode.Create("JFK"), Address.Create("New York",    "New York",    "United States")),
            Location.Create(AirportCode.Create("LAX"), Address.Create("Los Angeles", "California",  "United States")),
            Location.Create(AirportCode.Create("DFW"), Address.Create("Dallas",      "Texas",       "United States")),
            Location.Create(AirportCode.Create("MIA"), Address.Create("Miami",       "Florida",     "United States")),
            Location.Create(AirportCode.Create("BOM"), Address.Create("Mumbai",      "Maharashtra", "India")),
            Location.Create(AirportCode.Create("BLR"), Address.Create("Bengaluru",   "Karnataka",   "India")),
            Location.Create(AirportCode.Create("DEL"), Address.Create("New Delhi",   "Delhi",       "India"))
        );
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedRoutesAsync(IServiceProvider sp)
    {
        var ctx = sp.GetRequiredService<RoutingDbContext>();
        ctx.Routes.AddRange(
            RoutingDomain.Route.Create(RoutingDomain.RouteId.From(R1), RoutingDomain.AirportCode.Create("ORD"), RoutingDomain.AirportCode.Create("JFK")),
            RoutingDomain.Route.Create(RoutingDomain.RouteId.From(R2), RoutingDomain.AirportCode.Create("JFK"), RoutingDomain.AirportCode.Create("LAX")),
            RoutingDomain.Route.Create(RoutingDomain.RouteId.From(R3), RoutingDomain.AirportCode.Create("LAX"), RoutingDomain.AirportCode.Create("DFW")),
            RoutingDomain.Route.Create(RoutingDomain.RouteId.From(R4), RoutingDomain.AirportCode.Create("DFW"), RoutingDomain.AirportCode.Create("MIA")),
            RoutingDomain.Route.Create(RoutingDomain.RouteId.From(R5), RoutingDomain.AirportCode.Create("MIA"), RoutingDomain.AirportCode.Create("ORD"))
        );
        await ctx.SaveChangesAsync();
    }

    private static async Task SeedAirlinesAndFlightsAsync(IServiceProvider sp)
    {
        var ctx = sp.GetRequiredService<TimetableDbContext>();

        var airlines = new (Guid Id, string Name)[]
        {
            (A1, "United Airlines"),
            (A2, "American Airlines"),
            (A3, "Delta Air Lines"),
            (A4, "Southwest Airlines"),
            (A5, "JetBlue Airways")
        };

        foreach (var (id, name) in airlines)
            ctx.Airlines.Add(Airline.Create(AirlineId.From(id), name));

        var flights = new (Guid Fid, Guid Aid, Guid Rid, DateTime Dep, DateTime Arr, string Status)[]
        {
            (F1, A1, R1, new DateTime(2026, 6, 20,  8,  0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 20, 11, 30, 0, DateTimeKind.Utc), "Scheduled"),
            (F2, A2, R2, new DateTime(2026, 6, 20, 13,  0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 20, 16, 45, 0, DateTimeKind.Utc), "Scheduled"),
            (F3, A3, R3, new DateTime(2026, 6, 21,  9, 30, 0, DateTimeKind.Utc), new DateTime(2026, 6, 21, 14,  0, 0, DateTimeKind.Utc), "Delayed"),
            (F4, A4, R4, new DateTime(2026, 6, 21, 15,  0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 21, 18, 30, 0, DateTimeKind.Utc), "Scheduled"),
            (F5, A5, R5, new DateTime(2026, 6, 22,  7,  0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 22, 11,  0, 0, DateTimeKind.Utc), "InAir")
        };

        foreach (var (fid, aid, rid, dep, arr, status) in flights)
            ctx.Flights.Add(Flight.Create(FlightId.From(fid), aid, rid, FlightSchedule.Create(dep, arr), status));

        var locationMap = new Dictionary<string, (string City, string State, string Country)>
        {
            ["ORD"] = ("Chicago",     "Illinois",    "United States"),
            ["JFK"] = ("New York",    "New York",    "United States"),
            ["LAX"] = ("Los Angeles", "California",  "United States"),
            ["DFW"] = ("Dallas",      "Texas",       "United States"),
            ["MIA"] = ("Miami",       "Florida",     "United States"),
            ["BOM"] = ("Mumbai",      "Maharashtra", "India"),
            ["BLR"] = ("Bengaluru",   "Karnataka",   "India"),
            ["DEL"] = ("New Delhi",   "Delhi",       "India")
        };

        var routeMap = new Dictionary<Guid, (string From, string To)>
        {
            [R1] = ("ORD", "JFK"),
            [R2] = ("JFK", "LAX"),
            [R3] = ("LAX", "DFW"),
            [R4] = ("DFW", "MIA"),
            [R5] = ("MIA", "ORD")
        };

        var airlineNameMap = airlines.ToDictionary(a => a.Id, a => a.Name);

        foreach (var (fid, aid, rid, dep, arr, status) in flights)
        {
            var (fromCode, toCode) = routeMap[rid];
            var (fc, fs, fcountry) = locationMap[fromCode];
            var (tc, ts, tcountry) = locationMap[toCode];

            ctx.FlightViews.Add(new FlightView
            {
                FlightId        = fid,
                AirlineId       = aid,
                Airline         = airlineNameMap[aid],
                RouteId         = rid,
                FromAirportCode = fromCode,
                FromCity        = fc,
                FromState       = fs,
                FromCountry     = fcountry,
                ToAirportCode   = toCode,
                ToCity          = tc,
                ToState         = ts,
                ToCountry       = tcountry,
                DepartureUtc    = dep,
                ArrivalUtc      = arr,
                Status          = status
            });
        }

        await ctx.SaveChangesAsync();
    }
}
