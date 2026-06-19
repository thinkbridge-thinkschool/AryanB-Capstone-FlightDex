namespace FlightDex.SharedKernel.Pagination;

/// <summary>
/// Page number/size input contract for paginated queries. Values are normalised on
/// construction so an over-large or non-positive page size from an untrusted caller
/// can never reach the database — the cap (<see cref="MaxPageSize"/>) bounds the work
/// any single request can ask the read store to do, closing a cheap DoS vector.
/// </summary>
public sealed record PagedRequest
{
    /// <summary>Hard upper bound on rows returned per page, regardless of caller input.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Page size used when the caller omits one or sends a non-positive value.</summary>
    public const int DefaultPageSize = 20;

    public int PageNumber { get; }
    public int PageSize { get; }

    public PagedRequest(int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize switch
        {
            < 1           => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _             => pageSize
        };
    }
}
