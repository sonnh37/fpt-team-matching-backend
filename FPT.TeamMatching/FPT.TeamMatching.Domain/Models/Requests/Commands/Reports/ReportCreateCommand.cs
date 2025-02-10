using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reports;

public class ReportCreateCommand : CreateCommand
{
    public Guid? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Question { get; set; }

    public string? Document { get; set; }
}