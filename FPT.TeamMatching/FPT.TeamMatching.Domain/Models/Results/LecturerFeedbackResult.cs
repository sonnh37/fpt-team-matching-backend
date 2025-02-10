using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class LecturerFeedbackResult : BaseResult
{
    public Guid? LecturerId { get; set; }

    public Guid? ReportId { get; set; }

    public string? Content { get; set; }
}