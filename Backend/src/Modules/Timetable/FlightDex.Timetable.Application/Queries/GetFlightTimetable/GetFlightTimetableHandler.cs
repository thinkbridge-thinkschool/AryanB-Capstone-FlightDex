using FlightDex.SharedKernel.Pagination;
using FlightDex.Timetable.Application.Contracts;
using FlightDex.Timetable.Domain.ReadModels;

namespace FlightDex.Timetable.Application.Queries.GetFlightTimetable;

public interface IFlightViewReadStore
{
    Task<PagedResult<FlightView>> QueryAsync(GetFlightTimetableQuery query, CancellationToken cancellationToken = default);
    Task<FlightView?> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default);
}

public sealed class GetFlightTimetableHandler
{
    private readonly IFlightViewReadStore _readStore;

    public GetFlightTimetableHandler(IFlightViewReadStore readStore) => _readStore = readStore;

    public async Task<PagedResult<FlightSummaryDto>> HandleAsync(
        GetFlightTimetableQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _readStore.QueryAsync(query, cancellationToken);

        var dtos = result.Items.Select(v => new FlightSummaryDto(
            v.FlightId,
            v.Airline,
            v.FromCity,
            v.ToCity,
            v.DepartureUtc,
            v.ArrivalUtc,
            v.Status)).ToList();

        return new PagedResult<FlightSummaryDto>(dtos, result.PageNumber, result.PageSize, result.TotalCount);
    }
}
