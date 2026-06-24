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

        return new PagedResult<FlightListItem>(
            page.Items.Select(FlightListItem.FromDomain).ToList(),
            page.Page, page.PageSize, page.TotalCount);
    }
}
