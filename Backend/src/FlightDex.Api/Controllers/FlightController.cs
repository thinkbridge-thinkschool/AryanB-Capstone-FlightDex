using System.Globalization;
using FlightDex.Flights.Application;
using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Queries.GetFlightByCode;
using FlightDex.Flights.Application.Queries.GetFlights;
using FlightDex.Flights.Domain;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

[ApiController]
[Route("flight")]
public sealed class FlightController(IQueryDispatcher dispatcher) : ControllerBase
{
    private const int DefaultPageSize = 30;
    private const int MaxPageSize = 100;

    /// <summary>
    /// Departures or arrivals for an airport, paged and sorted by time.
    ///
    /// Direction is chosen from the query string:
    ///   ?at=BLR&amp;to            → all departures from BLR
    ///   ?at=BLR&amp;to=LAX        → departures from BLR to LAX
    ///   ?at=BLR&amp;from          → all arrivals at BLR
    ///   ?at=BLR&amp;from=LAX      → arrivals at BLR from LAX
    ///   ?deptTime_After=0800&amp;deptTime_Before=1200  → departures in a time window
    ///   ?arrTime_After=0800&amp;arrTime_Before=1200    → arrivals in a time window
    /// With neither to/from nor a time filter, departures are returned.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFlights(
        [FromQuery] string? at,
        [FromQuery] string? to,
        [FromQuery] string? from,
        [FromQuery] string? deptTime_After,
        [FromQuery] string? deptTime_Before,
        [FromQuery] string? arrTime_After,
        [FromQuery] string? arrTime_Before,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrWhiteSpace(at) && !ServedAirports.IsServed(at))
            return Problem($"'at' must be one of {string.Join(", ", ServedAirports.All)}.", statusCode: StatusCodes.Status400BadRequest);

        // Presence of the key (even without a value) selects the direction.
        var hasFrom = Request.Query.ContainsKey("from");
        var hasArrTime = !string.IsNullOrWhiteSpace(arrTime_After) || !string.IsNullOrWhiteSpace(arrTime_Before);
        var direction = hasFrom || hasArrTime ? FlightDirection.Arrival : FlightDirection.Departure;

        // The "to"/"from" value may be a destination/origin code OR a city name.
        var counterpartTerm = direction == FlightDirection.Arrival ? from : to;
        counterpartTerm = string.IsNullOrWhiteSpace(counterpartTerm) ? null : counterpartTerm.Trim();

        var afterRaw = direction == FlightDirection.Arrival ? arrTime_After : deptTime_After;
        var beforeRaw = direction == FlightDirection.Arrival ? arrTime_Before : deptTime_Before;

        if (!TryParseClock(afterRaw, out var timeAfter))
            return Problem("Invalid time filter; use HHMM (e.g. 0830).", statusCode: StatusCodes.Status400BadRequest);
        if (!TryParseClock(beforeRaw, out var timeBefore))
            return Problem("Invalid time filter; use HHMM (e.g. 0830).", statusCode: StatusCodes.Status400BadRequest);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        var spec = new FlightQuerySpec(
            Direction: direction,
            Airport: string.IsNullOrWhiteSpace(at) ? null : at.Trim().ToUpperInvariant(),
            CounterpartTerm: counterpartTerm,
            TimeAfter: timeAfter,
            TimeBefore: timeBefore,
            Page: page,
            PageSize: pageSize);

        var result = await dispatcher.DispatchAsync(new GetFlightsQuery(spec), cancellationToken);
        return Ok(result);
    }

    /// <summary>Full details for a flight code (the "click a flight" view).</summary>
    [HttpGet("{flightCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFlightByCode(string flightCode, CancellationToken cancellationToken)
    {
        var details = await dispatcher.DispatchAsync(
            new GetFlightByCodeQuery(flightCode.Trim().ToUpperInvariant()), cancellationToken);

        if (details.Count == 0)
            return NotFound();

        // A code is normally a single leg; unwrap the common case for the client.
        return Ok(details.Count == 1 ? (object)details[0] : details);
    }

    /// <summary>Parses an optional "HHMM" (or "HH:mm") clock filter. Null/blank → no filter.</summary>
    private static bool TryParseClock(string? raw, out TimeOnly? time)
    {
        time = null;
        if (string.IsNullOrWhiteSpace(raw)) return true;

        var s = raw.Trim();
        string[] formats = ["HHmm", "HH\\:mm", "H\\:mm"];
        if (TimeOnly.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            time = parsed;
            return true;
        }
        return false;
    }
}
