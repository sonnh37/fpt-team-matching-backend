using System.ComponentModel.DataAnnotations;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Utilities;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

public abstract class BaseQuery
{
}

public class GetQueryableQuery : BaseQuery
{
    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }


    [Required] public bool IsPagination { get; set; } = Const.IsPagination;

    public Guid? Id { get; set; }

    public string? CreatedBy { get; set; }

    public string? UpdatedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public int PageNumber { get; set; } = Const.PageNumberDefault;

    public int PageSize { get; set; } = Const.PageSizeDefault;

    public string? SortField { get; set; } = Const.SortFieldDefault;

    public SortOrder? SortOrder { get; set; } = Const.SortOrderDefault;
}

public class GetByIdQuery : BaseQuery
{
    public Guid? Id { get; set; }
}

public class GetAllQuery : GetQueryableQuery
{
}