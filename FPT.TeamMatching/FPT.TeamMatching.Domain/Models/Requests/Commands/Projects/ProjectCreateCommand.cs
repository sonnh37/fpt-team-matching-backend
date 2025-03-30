using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using Newtonsoft.Json;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;

public class ProjectCreateCommand : CreateCommand
{
    public Guid? LeaderId { get; set; }

    public Guid? IdeaId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }
}