using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TeamMembers;

public class TeamMemberUpdateCommand : UpdateCommand
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public TeamMemberRole? Role { get; set; }

    public DateTimeOffset? JoinDate { get; set; }

    public DateTimeOffset? LeaveDate { get; set; }

    public TeamMemberStatus? Status { get; set; }

    public MentorConclusionOptions? MentorConclusion { get; set; }

    public string? Attitude { get; set; }

    public string? CommentDefense1 { get; set; }

    public string? CommentDefense2 { get; set; }
}