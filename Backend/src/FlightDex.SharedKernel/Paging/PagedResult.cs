namespace FlightDex.SharedKernel.Paging;

/// <summary>Generic paginated response envelope.</summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount);
