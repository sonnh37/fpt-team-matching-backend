using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.ProjectActivities;

public class ProjectActivityCreateCommand : CreateCommand
{
    public Guid? ProjectId { get; set; }

    public string? Content { get; set; }
}