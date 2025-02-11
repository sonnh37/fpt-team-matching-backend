using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Project : BaseEntity
{
    public Guid? LeaderId { get; set; }

    public string? TeamName { get; set; }

    public string? Name { get; set; }

    public ProjectType? Type { get; set; }

    public string? Description { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }
    
    public virtual User? Leader { get; set; }

    public virtual ICollection<InvitationUser> InvitationUsers { get; set; } = new List<InvitationUser>();

    public virtual ICollection<ProjectActivity> ProjectActivities { get; set; } = new List<ProjectActivity>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}