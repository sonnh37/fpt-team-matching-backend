using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class FeedbackResult : BaseResult
{
    public Guid? ReviewId { get; set; }

    public string? Content { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public ReviewResult? Review { get; set; }
}