using FlightDex.Booking.Application;
using FlightDex.Booking.Infrastructure;
using FlightDex.Flights.Application;
using FlightDex.Flights.Infrastructure;
using FlightDex.SharedKernel.Cqrs;

var builder = WebApplication.CreateBuilder(args);

// TODO: CORS for the Angular client.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// CQRS + the Flights (Timetable) slice.
builder.Services.AddCqrs();
builder.Services.AddFlightsApplication();
builder.Services.AddFlightsInfrastructure(builder.Configuration);

// The Booking slice: users, auth, tickets.
builder.Services.AddBookingApplication();
builder.Services.AddBookingInfrastructure(builder.Configuration);

// TODO: JWT bearer authentication from the "Jwt" config section.
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// TODO: EnsureCreated for each module's SQLite DB and seed the timetable on startup.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
