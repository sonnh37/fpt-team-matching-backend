namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;

public class TeamMemberAddRangeCommand
{
    public ICollection<TeamMemberCreateCommand> TeamMembers { get; set; }
    public Guid TopicId {get; set;}
    public Guid ProjectId {get; set;}
}