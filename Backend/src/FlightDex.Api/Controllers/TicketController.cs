using System.Globalization;
using FlightDex.Api.Contracts;
using FlightDex.Booking.Application.Commands.BookTicket;
using FlightDex.Booking.Application.Commands.CancelTicket;
using FlightDex.Booking.Application.Dtos;
using FlightDex.Booking.Application.Queries.GetMyTickets;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

/// <summary>
/// Booking, listing and cancelling tickets. Every action requires a valid bearer token —
/// the user can only ever see and touch their own tickets.
/// </summary>
[ApiController]
[Route("ticket")]
[Authorize]
public sealed class TicketController(
    ICommandDispatcher commands,
    IQueryDispatcher queries) : ControllerBase
{
    /// <summary>Books a ticket for the signed-in user.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Book([FromBody] BookTicketRequest request, CancellationToken cancellationToken)
    {
        if (!DateOnly.TryParseExact(request.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return Problem("Date must be 'yyyy-MM-dd'.", statusCode: StatusCodes.Status400BadRequest);

        if (!TimeOnly.TryParseExact(request.Time, ["HH\\:mm", "HH\\:mm\\:ss"], CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            return Problem("Time must be 'HH:mm'.", statusCode: StatusCodes.Status400BadRequest);

        if (request.Origin is null || request.Destination is null
            || string.IsNullOrWhiteSpace(request.Origin.Code) || string.IsNullOrWhiteSpace(request.Destination.Code))
            return Problem("Origin and destination are required.", statusCode: StatusCodes.Status400BadRequest);

        var command = new BookTicketCommand(
            User.GetUserId(),
            date,
            time,
            request.Origin.Code, request.Origin.Airport, request.Origin.City,
            request.Destination.Code, request.Destination.Airport, request.Destination.City);

        var ticket = await commands.DispatchAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetMine), new { }, ticket);
    }

    /// <summary>The signed-in user's tickets, most recent first.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        var tickets = await queries.DispatchAsync(new GetMyTicketsQuery(User.GetUserId()), cancellationToken);
        return Ok(tickets);
    }

    /// <summary>Cancels one of the signed-in user's tickets.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var cancelled = await commands.DispatchAsync(
            new CancelTicketCommand(User.GetUserId(), id), cancellationToken);

        return cancelled ? NoContent() : NotFound();
    }
}
