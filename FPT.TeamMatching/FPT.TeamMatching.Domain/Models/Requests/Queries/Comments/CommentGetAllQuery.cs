using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Comments;

public class CommentGetAllQuery : GetQueryableQuery
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public string? Content { get; set; }
}