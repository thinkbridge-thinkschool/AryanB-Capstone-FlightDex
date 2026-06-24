using FlightDex.Flights.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

[ApiController]
[Route("airports")]
public sealed class AirportController(IAirportSuggestionCache cache) : ControllerBase
{
    /// <summary>
    /// Type-ahead suggestions — every unique airport code, airport name and city —
    /// served from the Locations lookup table rather than scanning the Flights table.
    /// </summary>
    [HttpGet("suggestions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSuggestions(CancellationToken cancellationToken)
    {
        var suggestions = await cache.GetAllAsync(cancellationToken);
        return Ok(suggestions);
    }
}
