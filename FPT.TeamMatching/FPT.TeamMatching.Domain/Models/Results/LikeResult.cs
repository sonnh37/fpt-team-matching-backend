using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class LikeResult : BaseResult
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public virtual BlogResult? Blog { get; set; }

    public virtual UserResult? User { get; set; }
}