using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Responses;

public class QueryResult
{
    public QueryResult()
    {
    }

    public QueryResult(GetQueryableQuery query, IEnumerable<object>? results = null, int? totalRecords = null)
    {
        Results = results ?? [];
        IsPagination = query.IsPagination;
        if (!query.IsPagination) return;
        
        PageNumber = query.PageNumber;
        PageSize = query.PageSize;
        SortField = query.SortField;
        SortOrder = query.SortOrder;
        TotalRecords = totalRecords;
        if (totalRecords != null) TotalPages = (int)Math.Ceiling((decimal)(totalRecords / (double)query.PageSize));
    }

    public IEnumerable<object>? Results { get; }

    public int? TotalPages { get; protected set; }
    public int? TotalRecords { get; protected set; }

    public int? PageNumber { get; protected set; }

    public int? PageSize { get; protected set; }
    
    public bool IsPagination { get; protected set; }

    public string? SortField { get; protected set; }

    public SortOrder? SortOrder { get; protected set; }
}
