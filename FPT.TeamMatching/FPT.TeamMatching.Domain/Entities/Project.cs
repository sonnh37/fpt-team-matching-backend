using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Project : BaseEntity
{
    public Guid? LeaderId { get; set; }

    public Guid? IdeaId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public int? DefenseStage { get; set; }

    public virtual User? Leader { get; set; }

    public virtual Idea? Idea { get; set; }

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
    
    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<MentorIdeaRequest> MentorIdeaRequests { get; set; } = new List<MentorIdeaRequest>();

    public virtual ICollection<CapstoneSchedule> CapstoneSchedules { get; set; } = new List<CapstoneSchedule>();
}