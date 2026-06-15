namespace FlightDex.SharedKernel.Pagination;

/// <summary>Generic paginated response envelope.</summary>
public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    public int TotalPages => throw new NotImplementedException();

    public bool HasNextPage => throw new NotImplementedException();

    public bool HasPreviousPage => throw new NotImplementedException();
}
