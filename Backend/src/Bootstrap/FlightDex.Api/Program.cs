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

// ── Input / transport limits ────────────────────────────────────────────────────
// The API only ever receives tiny JSON requests, so cap the body and stop Kestrel
// advertising itself — both are cheap hardening of the public surface (input limits +
// tech-stack non-disclosure flagged by the ZAP baseline).
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;                  // don't leak "Server: Kestrel"
    options.Limits.MaxRequestBodySize = 64 * 1024;    // 64 KB is ample for our payloads
});

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

// ── Security response headers ────────────────────────────────────────────────────
// Closes the OWASP ZAP baseline findings (missing CSP, X-Content-Type-Options,
// anti-clickjacking, referrer policy, HSTS). The API serves JSON only, so the CSP is
// locked down to nothing and framing is denied outright. Runs first, on every response.
app.Use(async (context, next) =>
{
    var headers = context.Response.Headers;
    headers["X-Content-Type-Options"]    = "nosniff";
    headers["X-Frame-Options"]           = "DENY";
    headers["Content-Security-Policy"]   = "default-src 'none'; frame-ancestors 'none'";
    headers["Referrer-Policy"]           = "no-referrer";
    headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
    headers["Cross-Origin-Resource-Policy"] = "same-origin";
    headers["Cache-Control"]             = "no-store";
    await next();
});

await DatabaseSeeder.SeedAsync(app.Services);

app.MapFlightEndpoints();

app.Run();
