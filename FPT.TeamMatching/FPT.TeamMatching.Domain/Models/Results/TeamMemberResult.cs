using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class TeamMemberResult : BaseResult
{
    public Guid? UserId { get; set; }
    
    public Guid? ProjectId { get; set; }

    public TeamMemberRole? Role { get; set; }

    public DateTimeOffset? JoinDate { get; set; }

    public DateTimeOffset? LeaveDate { get; set; }
    
    public UserResult? User { get; set; }
    
    public ProjectResult? Project { get; set; }
    
    public ICollection<RateResult> Rates { get; set; } = new List<RateResult>();

}