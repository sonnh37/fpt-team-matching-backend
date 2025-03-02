using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ReviewResult : BaseResult
{
    public Guid? ProjectId { get; set; }

    public int Number { get; set; }

    public string? Description { get; set; }

    public string? FileUpload { get; set; }

    public string? Reviewer1 { get; set; }

    public string? Reviewer2 { get; set; }

    public ProjectResult? Project { get; set; }

    public ICollection<FeedbackResult> Feedbacks { get; set; } = new List<FeedbackResult>();
}