using FlightDex.SharedKernel.Pagination;
using FlightDex.Timetable.Application.Contracts;
using FlightDex.Timetable.Domain.ReadModels;

namespace FlightDex.Timetable.Application.Queries.GetFlightTimetable;

/// <summary>Read-side port over the FlightView store; implemented in the Infrastructure layer.</summary>
public interface IFlightViewReadStore
{
    Task<PagedResult<FlightView>> QueryAsync(GetFlightTimetableQuery query, CancellationToken cancellationToken = default);

    Task<FlightView?> GetByFlightIdAsync(Guid flightId, CancellationToken cancellationToken = default);
}

/// <summary>Returns a page of flight summaries from the FlightView read model.</summary>
public sealed class GetFlightTimetableHandler
{
    private readonly IFlightViewReadStore _readStore;

    public GetFlightTimetableHandler(IFlightViewReadStore readStore) => _readStore = readStore;

    public Task<PagedResult<FlightSummaryDto>> HandleAsync(
        GetFlightTimetableQuery query,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();
}
