using FlightDex.Api.Endpoints;
using FlightDex.Api.Modules;

// App entry point: wires the bounded-context modules into the host and starts the server.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

app.MapFlightEndpoints();

app.Run();
