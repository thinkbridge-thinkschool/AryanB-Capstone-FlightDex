using FlightDex.Flights.Application.Abstractions;
using FlightDex.Flights.Application.Dtos;
using FlightDex.SharedKernel.Cqrs;
using FlightDex.SharedKernel.Paging;

namespace FlightDex.Flights.Application.Queries.GetFlights;

internal sealed class GetFlightsQueryHandler(IFlightRepository repository)
    : IQueryHandler<GetFlightsQuery, PagedResult<FlightListItem>>
{
    public async Task<PagedResult<FlightListItem>> HandleAsync(
        GetFlightsQuery query, CancellationToken cancellationToken = default)
    {
        var page = await repository.GetPagedAsync(query.Spec, cancellationToken);

        var items = page.Items.Select(FlightListItem.FromDomain).ToList();
        return new PagedResult<FlightListItem>(items, page.Page, page.PageSize, page.TotalCount);
    }
}
