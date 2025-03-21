using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class CommentResult : BaseResult
{
    public Guid? BlogId { get; set; }

    public Guid? UserId { get; set; }

    public string? Content { get; set; }

    public  User? User { get; set; }
}