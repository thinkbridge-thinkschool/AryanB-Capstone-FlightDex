using System.Diagnostics;
using System.Text.Json;
using FlightDex.Flights.Domain;
using FlightDex.Flights.Infrastructure.Persistence;
using FlightDex.IntegrationTests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FlightDex.IntegrationTests;

/// <summary>
/// Latency benchmark for the hottest path — GET /flight?at=BLR (the default departures
/// board). Measured in-process so it isolates the controller → CQRS → EF Core → SQLite
/// cost from the network. Excluded from the normal/CI run via the "Perf" trait; run with
///   dotnet test --filter Category=Perf
/// Writes percentiles to the file named by the PERF_OUT env var (or a temp file).
/// </summary>
[Trait("Category", "Perf")]
public sealed class PerfTests
{
    private const string HotPath = "/flight?at=BLR";
    private const int SeededDepartures = 1000;   // realistic board size to sort/page/count over
    private const int Warmup = 500;
    private const int Iterations = 5000;

    [Fact]
    public async Task Measure_hot_path_p99()
    {
        await using var factory = new FlightDexApiFactory();
        await ((IAsyncLifetime)factory).InitializeAsync();   // wipes + seeds the known set
        await SeedManyAsync(factory, SeededDepartures);
        var client = factory.CreateClient();

        // Warm up: JIT, EF model build, first-query compilation, count-cache priming.
        for (var i = 0; i < Warmup; i++)
            (await client.GetAsync(HotPath)).EnsureSuccessStatusCode();

        var samples = new double[Iterations];
        for (var i = 0; i < Iterations; i++)
        {
            var start = Stopwatch.GetTimestamp();
            var response = await client.GetAsync(HotPath);
            response.EnsureSuccessStatusCode();
            await response.Content.ReadAsStringAsync();
            samples[i] = Stopwatch.GetElapsedTime(start).TotalMilliseconds;
        }

        Array.Sort(samples);
        var result = new
        {
            path = HotPath,
            seededRows = SeededDepartures,
            iterations = Iterations,
            p50_ms = Percentile(samples, 50),
            p95_ms = Percentile(samples, 95),
            p99_ms = Percentile(samples, 99),
            max_ms = samples[^1],
            mean_ms = samples.Average(),
        };

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        var outPath = Environment.GetEnvironmentVariable("PERF_OUT")
            ?? Path.Combine(Path.GetTempPath(), "flightdex-perf.json");
        await File.WriteAllTextAsync(outPath, json);
        Console.WriteLine($"[PERF] {json}");

        Assert.True(result.p99_ms > 0);   // sanity; the numbers themselves are the artifact
    }

    private static double Percentile(double[] sorted, int p)
    {
        var rank = (int)Math.Ceiling(p / 100.0 * sorted.Length);
        return sorted[Math.Clamp(rank - 1, 0, sorted.Length - 1)];
    }

    private static async Task SeedManyAsync(FlightDexApiFactory factory, int count)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<FlightsDbContext>();
        var extra = Enumerable.Range(0, count).Select(i =>
        {
            var time = new TimeOnly((i % 24), (i % 60));
            return new Flight("BLR", FlightDirection.Departure, time, "AI", "Air India",
                "Indira Gandhi International Airport", "DEL", "Delhi", $"AI{1000 + i}", "2h 0m");
        });
        db.Flights.AddRange(extra);
        await db.SaveChangesAsync();
    }
}
