using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TeamMembers;

public class TeamMemberGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public TeamMemberRole? Role { get; set; }

    public DateTimeOffset? JoinDate { get; set; }

    public DateTimeOffset? LeaveDate { get; set; }

    public TeamMemberStatus? Status { get; set; }

    public MentorConclusionOptions? MentorConclusion { get; set; }

    public string? Attitude { get; set; }

}