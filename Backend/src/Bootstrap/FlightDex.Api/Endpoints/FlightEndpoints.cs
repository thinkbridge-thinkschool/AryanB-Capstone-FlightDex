using FlightDex.SharedKernel.Pagination;
using FlightDex.Timetable.Application.Queries.GetFlightDetails;
using FlightDex.Timetable.Application.Queries.GetFlightTimetable;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Endpoints;

public static class FlightEndpoints
{
    public static IEndpointRouteBuilder MapFlightEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/flight", async (
            [FromServices] GetFlightTimetableHandler handler,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 15,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? from = null,
            [FromQuery] string? to = null,
            CancellationToken ct = default) =>
        {
            var sort = sortBy?.ToLowerInvariant() switch
            {
                "arrival"  => FlightSortBy.Arrival,
                "location" => FlightSortBy.Location,
                "airline"  => FlightSortBy.Airline,
                _          => FlightSortBy.Departure
            };

            var query = new GetFlightTimetableQuery(
                new PagedRequest(page, pageSize),
                DepartureAirportCode: from,
                ArrivalAirportCode: to,
                SortBy: sort);

            var result = await handler.HandleAsync(query, ct);

            return Results.Ok(new
            {
                result.PageNumber,
                result.PageSize,
                result.TotalCount,
                result.TotalPages,
                result.HasNextPage,
                result.HasPreviousPage,
                Items = result.Items.Select(f => new
                {
                    f.FlightId,
                    f.Airline,
                    From = f.FromCity,
                    To   = f.ToCity,
                    Departure = f.DepartureUtc.ToString("HHmm ddMM"),
                    Arrival   = f.ArrivalUtc.ToString("HHmm ddMM"),
                    f.Status
                })
            });
        });

        app.MapGet("/flight/{flightId:guid}", async (
            Guid flightId,
            [FromServices] GetFlightDetailsHandler handler,
            CancellationToken ct) =>
        {
            var dto = await handler.HandleAsync(new GetFlightDetailsQuery(flightId), ct);
            if (dto is null) return Results.NotFound();

            return Results.Ok(new
            {
                dto.FlightId,
                dto.Airline,
                From = new
                {
                    AirportCode  = dto.FromAirportCode,
                    City         = dto.FromCity,
                    State        = dto.FromState,
                    Country      = dto.FromCountry,
                    LocationName = $"{dto.FromCity}, {dto.FromState}, {dto.FromCountry}"
                },
                To = new
                {
                    AirportCode  = dto.ToAirportCode,
                    City         = dto.ToCity,
                    State        = dto.ToState,
                    Country      = dto.ToCountry,
                    LocationName = $"{dto.ToCity}, {dto.ToState}, {dto.ToCountry}"
                },
                Departure = dto.DepartureUtc.ToString("HHmm ddMM"),
                Arrival   = dto.ArrivalUtc.ToString("HHmm ddMM"),
                dto.Status
            });
        });

        return app;
    }
}
