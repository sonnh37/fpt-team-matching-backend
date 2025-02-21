using CloudinaryDotNet.Actions;

namespace FPT.TeamMatching.Domain.Models.Results;

public class SemesterResult : BaseResult
{
    public string? SemesterCode { get; set; }

    public string? SemesterName { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public virtual ICollection<IdeaResult> Ideas { get; set; } = new List<IdeaResult>();
}