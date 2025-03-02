using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Likes;

public class LikeGetAllQuery : GetQueryableQuery
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }
}