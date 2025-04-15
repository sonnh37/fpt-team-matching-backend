using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;

public class ProjectUpdateCommand : UpdateCommand
{
    public Guid? LeaderId { get; set; }

    public Guid? TopicId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public int? DefenseStage { get; set; }
}