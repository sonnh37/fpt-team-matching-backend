using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class TeamMember : BaseEntity
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public TeamMemberRole? Role { get; set; }

    public DateTimeOffset? JoinDate { get; set; }

    public DateTimeOffset? LeaveDate { get; set; }
    
    public TeamMemberStatus? Status { get; set; }

    public MentorConclusionOptions? MentorConclusion {  get; set; }

    public string? Attitude { get; set; }

    public string? CommentDefense1 { get; set; }

    public string? CommentDefense2 { get; set; }

    public virtual Project? Project { get; set; }
    
    public virtual User? User { get; set; }
    
    public virtual ICollection<Rate> RateBys { get; set; } = new List<Rate>();
    
    public virtual ICollection<Rate> RateFors { get; set; } = new List<Rate>();
}