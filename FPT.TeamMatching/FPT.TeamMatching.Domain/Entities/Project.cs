using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Project : BaseEntity
{
    public Guid? LeaderId { get; set; }

    public Guid? IdeaId { get; set; }

    public string? TeamName { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public DateTimeOffset? StartDate { get; set; }

    public DateTimeOffset? EndDate { get; set; }

    public virtual User? Leader { get; set; }

    public virtual Idea? Idea { get; set; }

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    
    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}