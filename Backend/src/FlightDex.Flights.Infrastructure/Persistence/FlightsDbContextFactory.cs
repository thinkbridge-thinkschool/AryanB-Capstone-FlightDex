using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FlightDex.Flights.Infrastructure.Persistence;

/// <summary>Design-time factory so EF tooling can construct the context outside the host.</summary>
public sealed class FlightsDbContextFactory : IDesignTimeDbContextFactory<FlightsDbContext>
{
    public FlightsDbContext CreateDbContext(string[] args)
    {
        // TODO: build options with a design-time SQLite connection string.
        throw new NotImplementedException();
    }
}
