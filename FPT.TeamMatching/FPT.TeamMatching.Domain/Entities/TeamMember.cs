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

    public virtual Project? Project { get; set; }
    
    public virtual User? User { get; set; }
    
    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();
}