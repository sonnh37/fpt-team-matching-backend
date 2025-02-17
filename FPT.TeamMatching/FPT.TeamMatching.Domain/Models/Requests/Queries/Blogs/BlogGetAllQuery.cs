using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Blogs;

public class BlogGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public BlogType? Type { get; set; }

    public int? Quantity { get; set; }
}