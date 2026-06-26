using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Domain;
using FlightDex.Flights.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FlightDex.IntegrationTests;

/// <summary>
/// Infrastructure-layer integration: the repository's LINQ is exercised against a real
/// (in-memory) SQLite engine, so the filtering, ordering, paging and the page-count cache
/// are tested as actual SQL — not an EF in-memory fake.
/// </summary>
public sealed class FlightRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly FlightsDbContext _db;
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

    public FlightRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();   // keep the in-memory db alive for the connection's lifetime
        _db = new FlightsDbContext(new DbContextOptionsBuilder<FlightsDbContext>()
            .UseSqlite(_connection).Options);
        _db.Database.EnsureCreated();
    }

    private FlightRepository Sut() => new(_db, _cache);

    private static Flight Dep(TimeOnly time, string code, string toCode) =>
        new("BLR", FlightDirection.Departure, time, code[..2], "Air India",
            $"{toCode} Airport", toCode, toCode == "DEL" ? "Delhi" : "Mumbai", code, "2h 0m");

    private async Task SeedAsync(params Flight[] flights)
    {
        _db.Flights.AddRange(flights);
        await _db.SaveChangesAsync();
    }

    private static FlightQuerySpec Spec(
        string? counterpart = null, string? flightCode = null, TimeOnly? after = null, TimeOnly? before = null,
        int page = 1, int pageSize = 30) =>
        new(FlightDirection.Departure, "BLR", counterpart, flightCode, after, before, page, pageSize);

    [Fact]
    public async Task Filters_by_direction_and_airport_and_orders_by_time()
    {
        await SeedAsync(
            Dep(new(14, 0), "AI3", "DEL"),
            Dep(new(6, 0), "AI1", "DEL"),
            Dep(new(9, 0), "AI2", "BOM"),
            new("BOM", FlightDirection.Departure, new(7, 0), "AI", "Air India", "X", "BLR", "Bengaluru", "AI9", "2h"),
            new("BLR", FlightDirection.Arrival, new(7, 0), "AI", "Air India", "X", "DEL", "Delhi", "AI8", "2h"));

        var result = await Sut().GetPagedAsync(Spec());

        Assert.Equal(3, result.TotalCount);                          // only BLR departures
        Assert.Equal(["AI1", "AI2", "AI3"], result.Items.Select(f => f.FlightCode));   // time-sorted
    }

    [Fact]
    public async Task Filters_by_counterpart_term()
    {
        await SeedAsync(Dep(new(6, 0), "AI1", "DEL"), Dep(new(9, 0), "AI2", "BOM"), Dep(new(10, 0), "AI3", "DEL"));

        var result = await Sut().GetPagedAsync(Spec(counterpart: "DEL"));

        Assert.Equal(2, result.TotalCount);
        Assert.All(result.Items, f => Assert.Equal("DEL", f.CounterpartCode));
    }

    [Fact]
    public async Task Applies_inclusive_time_window()
    {
        await SeedAsync(Dep(new(6, 0), "AI1", "DEL"), Dep(new(8, 0), "AI2", "DEL"), Dep(new(12, 0), "AI3", "DEL"));

        var result = await Sut().GetPagedAsync(Spec(after: new(8, 0), before: new(12, 0)));

        Assert.Equal(["AI2", "AI3"], result.Items.Select(f => f.FlightCode));
    }

    [Fact]
    public async Task Pages_results_while_reporting_the_full_total()
    {
        await SeedAsync(Enumerable.Range(0, 9).Select(i => Dep(new(6 + i, 0), $"AI{i}", "DEL")).ToArray());

        var page2 = await Sut().GetPagedAsync(Spec(page: 2, pageSize: 4));

        Assert.Equal(9, page2.TotalCount);
        Assert.Equal(4, page2.Items.Count);
        Assert.Equal(3, page2.TotalPages);
        Assert.Equal("AI4", page2.Items[0].FlightCode);   // 5th overall (skip 4)
    }

    [Fact]
    public async Task Caches_the_page_count_so_a_repeat_query_keeps_the_first_total()
    {
        await SeedAsync(Dep(new(6, 0), "AI1", "DEL"), Dep(new(9, 0), "AI2", "DEL"));
        var first = await Sut().GetPagedAsync(Spec());
        Assert.Equal(2, first.TotalCount);

        // Add a row matching the same filter; the cached count should not be recomputed.
        await SeedAsync(Dep(new(10, 0), "AI3", "DEL"));
        var second = await Sut().GetPagedAsync(Spec());

        Assert.Equal(2, second.TotalCount);   // still the cached total
    }

    [Fact]
    public async Task GetByFlightCode_returns_matching_rows()
    {
        await SeedAsync(Dep(new(6, 0), "AI1", "DEL"), Dep(new(9, 0), "AI2", "DEL"));

        var found = await Sut().GetByFlightCodeAsync("AI2");
        Assert.Single(found);
        Assert.Equal("AI2", found[0].FlightCode);

        Assert.Empty(await Sut().GetByFlightCodeAsync("NOPE"));
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
        _cache.Dispose();
    }
}
