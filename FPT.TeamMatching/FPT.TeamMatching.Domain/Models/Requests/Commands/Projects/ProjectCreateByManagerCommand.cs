using FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Projects;

public class ProjectCreateByManagerCommand
{
    public Guid? LeaderId { get; set; }
    public Guid? TopicId { get; set; }

    public ICollection<TeamMemberCreateCommand>? TeamMembers { get; set; }
}