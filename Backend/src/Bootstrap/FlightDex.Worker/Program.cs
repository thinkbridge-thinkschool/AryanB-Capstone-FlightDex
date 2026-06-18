using Azure.Monitor.OpenTelemetry.AspNetCore;
using FlightDex.Locations.Infrastructure;
using FlightDex.Messaging;
using FlightDex.Routing.Infrastructure;
using FlightDex.Timetable.Infrastructure;
using FlightDex.Worker;
using OpenTelemetry;
using OpenTelemetry.Resources;

// Emit Azure SDK (Service Bus) distributed-tracing spans and continue the `traceparent`
// carried in each consumed message, so the trace stitches API → worker. Must be set
// before any Azure client is created.
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);

var builder = WebApplication.CreateBuilder(args);

// ── Telemetry → Application Insights ────────────────────────────────────────────
// Distro auto-instruments HttpClient, SqlClient and the Azure SDK (Service Bus),
// so consume + DB spans flow to App Insights with no per-call code. Only wired when
// a connection string is present, so local runs stay telemetry-free.
var aiConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]
    ?? builder.Configuration["ApplicationInsights:ConnectionString"];
if (!string.IsNullOrWhiteSpace(aiConnectionString))
{
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(r => r.AddService("flightdex-worker"))   // → cloud_RoleName
        .UseAzureMonitor(o => o.ConnectionString = aiConnectionString);
}

// ── Domain modules (EF Core → SQL), reused from the API ─────────────────────────
builder.Services
    .AddLocationsModule(builder.Configuration)
    .AddRoutingModule(builder.Configuration)
    .AddTimetableModule(builder.Configuration);

// ── Service Bus consumer ────────────────────────────────────────────────────────
builder.Services.AddServiceBusMessaging(builder.Configuration);
builder.Services.AddScoped<FlightViewProjectionService>();

var serviceBusOptions = new ServiceBusOptions();
builder.Configuration.GetSection(ServiceBusOptions.SectionName).Bind(serviceBusOptions);
if (serviceBusOptions.IsConfigured)
    builder.Services.AddHostedService<ServiceBusEventConsumer>();

var app = builder.Build();

// Health probe so App Service keeps the (otherwise headless) worker running.
app.MapGet("/healthz", () => Results.Ok("healthy"));

app.Run();
