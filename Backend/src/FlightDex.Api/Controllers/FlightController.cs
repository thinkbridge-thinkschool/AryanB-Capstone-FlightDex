using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

/// <summary>Slice 1 — the timetable. Departures/arrivals lists and the single-flight detail view.</summary>
[ApiController]
[Route("flight")]
public sealed class FlightController(IQueryDispatcher dispatcher) : ControllerBase
{
    /// <summary>Departures or arrivals for an airport, paged and sorted by time.</summary>
    [HttpGet]
    public Task<IActionResult> GetFlights(
        [FromQuery] string? at,
        [FromQuery] string? to,
        [FromQuery] string? from,
        [FromQuery] string? deptTime_After,
        [FromQuery] string? deptTime_Before,
        [FromQuery] string? arrTime_After,
        [FromQuery] string? arrTime_Before,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        CancellationToken cancellationToken = default)
    {
        // TODO: validate, build FlightQuerySpec, dispatch GetFlightsQuery.
        throw new NotImplementedException();
    }

    /// <summary>Full details for a flight code (the "click a flight" view).</summary>
    [HttpGet("{flightCode}")]
    public Task<IActionResult> GetFlightByCode(string flightCode, CancellationToken cancellationToken)
    {
        // TODO: dispatch GetFlightByCodeQuery; 404 when empty.
        throw new NotImplementedException();
    }
}
