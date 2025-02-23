using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class BlogResult : BaseResult
{
    public Guid? UserId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public BlogType? Type { get; set; }
}