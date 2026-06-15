using FlightDex.Api;
using FlightDex.Api.Endpoints;
using FlightDex.Api.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(builder.Configuration);

var app = builder.Build();

await DatabaseSeeder.SeedAsync(app.Services);

app.MapFlightEndpoints();

app.Run();
