using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class Project : BaseEntity
{
    public Guid? OwnerId { get; set; }

    public string? TeamName { get; set; }

    public string? Name { get; set; }

    public string? Profession { get; set; }

    public string? Specialty { get; set; }

    public ProjectType? Type { get; set; }

    public string? Description { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public virtual ICollection<InvitationUser> InvitationUsers { get; set; } = new List<InvitationUser>();

    public virtual User? Owner { get; set; }

    public virtual ICollection<ProjectActivity> ProjectActivities { get; set; } = new List<ProjectActivity>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}