using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ReviewResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Question { get; set; }

    public string? Document { get; set; }
}