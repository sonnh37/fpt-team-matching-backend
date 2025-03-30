using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;

public class TeamCreateCommand : CreateCommand
{
    public string? TeamName { get; set; }

    public int? TeamSize { get; set; }
}