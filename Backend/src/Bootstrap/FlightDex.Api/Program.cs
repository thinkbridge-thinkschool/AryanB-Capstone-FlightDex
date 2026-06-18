using Azure.Monitor.OpenTelemetry.AspNetCore;
using FlightDex.Api;
using FlightDex.Api.Endpoints;
using FlightDex.Api.Modules;
using FlightDex.Messaging;
using OpenTelemetry;
using OpenTelemetry.Resources;

// Emit Azure SDK (Service Bus) distributed-tracing spans and propagate `traceparent`
// into published messages, so the trace stitches API → worker. Must be set before any
// Azure client is created.
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

// ── Telemetry → Application Insights ────────────────────────────────────────────
// The Azure Monitor distro auto-instruments ASP.NET Core, HttpClient, SqlClient and
// the Azure SDK (Service Bus). Only wired when a connection string is present so local
// runs stay telemetry-free.
var aiConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    ?? builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(aiConnectionString))
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService("flightdex-api"))   // → cloud_RoleName
        .UseAzureMonitor(o => o.ConnectionString = aiConnectionString);
}

builder.Services.AddModules(builder.Configuration);
builder.Services.AddServiceBusMessaging(builder.Configuration);

var app = builder.Build();

await DatabaseSeeder.SeedAsync(app.Services);

app.MapFlightEndpoints();

app.Run();
