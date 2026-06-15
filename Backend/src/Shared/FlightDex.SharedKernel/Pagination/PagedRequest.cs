namespace FlightDex.SharedKernel.Pagination;

/// <summary>Page number/size input contract for paginated queries.</summary>
public sealed record PagedRequest(int PageNumber = 1, int PageSize = 20);
