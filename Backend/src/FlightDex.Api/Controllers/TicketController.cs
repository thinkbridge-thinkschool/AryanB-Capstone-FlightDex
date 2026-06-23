using FlightDex.Api.Contracts;
using FlightDex.SharedKernel.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightDex.Api.Controllers;

/// <summary>
/// Slice 2 — booking, listing and cancelling tickets. Every action requires a valid bearer
/// token; the user can only ever see and touch their own tickets.
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
    public Task<IActionResult> Book([FromBody] BookTicketRequest request, CancellationToken cancellationToken)
    {
        // TODO: validate date/time/legs, dispatch BookTicketCommand(User.GetUserId(), ...).
        throw new NotImplementedException();
    }

    /// <summary>The signed-in user's tickets, most recent first.</summary>
    [HttpGet]
    public Task<IActionResult> GetMine(CancellationToken cancellationToken)
    {
        // TODO: dispatch GetMyTicketsQuery(User.GetUserId()).
        throw new NotImplementedException();
    }

    /// <summary>Cancels one of the signed-in user's tickets.</summary>
    [HttpDelete("{id:int}")]
    public Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        // TODO: dispatch CancelTicketCommand(User.GetUserId(), id); 204 / 404.
        throw new NotImplementedException();
    }
}
