using System.Text;
using FlightDex.Booking.Application;
using FlightDex.Booking.Infrastructure;
using FlightDex.Booking.Infrastructure.Persistence;
using FlightDex.Booking.Infrastructure.Security;
using FlightDex.Flights.Application;
using FlightDex.Flights.Infrastructure;
using FlightDex.Flights.Infrastructure.Persistence;
using FlightDex.Flights.Infrastructure.Persistence.Seeding;
using System.Text.Json.Serialization;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

const string AngularCorsPolicy = "AngularClient";
var angularOrigins = builder.Configuration.GetSection("Cors:AngularOrigins").Get<string[]>()
    ?? ["http://localhost:4200"];

builder.Services.AddCors(options => options.AddPolicy(AngularCorsPolicy, policy =>
    policy.WithOrigins(angularOrigins).AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddControllers()
    .AddJsonOptions(o =>
        o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

// CQRS + the Flights vertical slice.
builder.Services.AddCqrs();
builder.Services.AddFlightsApplication();
builder.Services.AddFlightsInfrastructure(builder.Configuration);

// The Booking slice: users, auth, tickets.
builder.Services.AddBookingApplication();
builder.Services.AddBookingInfrastructure(builder.Configuration);

// JWT bearer authentication. Validates tokens issued by JwtTokenService against the same
// "Jwt" settings used to sign them.
var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
    ?? throw new InvalidOperationException("The 'Jwt' configuration section is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Create each module's SQLite schema from its model (no migrations) and seed the timetable.
await using (var scope = app.Services.CreateAsyncScope())
{
    var flightsDb = scope.ServiceProvider.GetRequiredService<FlightsDbContext>();
    await flightsDb.Database.EnsureCreatedAsync();

    var bookingDb = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    await bookingDb.Database.EnsureCreatedAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<FlightTimetableSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(AngularCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
