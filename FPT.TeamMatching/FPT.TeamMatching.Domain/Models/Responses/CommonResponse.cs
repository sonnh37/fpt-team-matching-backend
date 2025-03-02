using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Responses;

public class LoginResponse
{
    public LoginResponse()
    {
    }

    public LoginResponse(string? token, string? expiration)
    {
        Token = token;
        Expiration = expiration;
    }

    public string? Token { get; set; }
    public string? Expiration { get; set; }
}

public class ResultResponse<TResult> where TResult : class
{
    public ResultResponse()
    {
    }

    public ResultResponse(TResult? result = null)
    {
        Result = result;
    }

    public TResult? Result { get; set; }
}

public class ResultsResponse<TResult> where TResult : class
{
    public ResultsResponse()
    {
    }

    public ResultsResponse(IEnumerable<TResult>? results = null)
    {
        Results = results ?? [];
        TotalRecords = results?.Count() ?? 0;
    }

    public IEnumerable<TResult>? Results { get; set; }

    public int TotalRecords { get; set; }
}

public class PagedResponse<TResult> where TResult : class
{
    public PagedResponse()
    {
    }

    public PagedResponse(GetQueryableQuery pagedQuery, IEnumerable<TResult>? results = null, int? totalOrigin = null)
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

    public IEnumerable<TResult>? Results { get; }

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