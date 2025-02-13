using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.LecturerFeedbacks;

public class LecturerFeedbackCreateCommand : CreateCommand
{
    public Guid? LecturerId { get; set; }

    public Guid? ReportId { get; set; }

    public string? Content { get; set; }
}