using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Responses;

public class PaginatedResult
{
    public PaginatedResult()
    {
    }

    public PaginatedResult(GetQueryableQuery pagedQuery, IEnumerable<object>? results = null, int? totalOrigin = null)
    {
        PageNumber = totalOrigin != null ? pagedQuery.PageNumber : null;
        PageSize = totalOrigin != null ? pagedQuery.PageSize : null;
        SortField = totalOrigin != null ? pagedQuery.SortField : null;
        SortOrder = totalOrigin != null ? pagedQuery.SortOrder : null;
        Results = results;
        TotalRecords = totalOrigin ?? results?.Count();
        TotalRecordsPerPage = totalOrigin != null ? results?.Count() : null;
        TotalPages = totalOrigin != null
            ? (int)Math.Ceiling((decimal)(totalOrigin / (double)pagedQuery.PageSize))
            : null;
    }

    public IEnumerable<object>? Results { get; }

    public int? TotalPages { get; protected set; }

    public int? TotalRecordsPerPage { get; protected set; }

    public int? TotalRecords { get; protected set; }

    public int? PageNumber { get; protected set; }

    public int? PageSize { get; protected set; }

    public string? SortField { get; protected set; }

    public SortOrder? SortOrder { get; protected set; }
}

public class ResultsTableResponse<TResult> where TResult : class
{
    public (List<TResult>?, int?)? Item { get; set; }
    public GetQueryableQuery? GetQueryableQuery { get; set; }
}