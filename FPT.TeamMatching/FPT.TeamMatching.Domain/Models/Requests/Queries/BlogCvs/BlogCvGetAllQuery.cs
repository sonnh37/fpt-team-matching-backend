using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.BlogCvs;

public class BlogCvGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public Guid? BlogId { get; set; }

    public string? FileCv { get; set; }
}