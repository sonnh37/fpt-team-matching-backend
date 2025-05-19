namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;

public class TeamMemberAddRangeCommand
{
    public ICollection<TeamMemberCreateCommand> TeamMembers { get; set; }
}