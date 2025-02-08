using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;

public class ProjectUpdateCommand : UpdateCommand
{
    public Guid? LeaderId { get; set; }

    public string? TeamName { get; set; }

    public string? Name { get; set; }

    public ProjectType? Type { get; set; }

    public string? Description { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }
}