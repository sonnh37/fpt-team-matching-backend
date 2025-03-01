using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ProjectResult : BaseResult
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

    public UserResult? Leader { get; set; }

    public IdeaResult? Idea { get; set; }

    public ICollection<BlogResult> Blogs { get; set; } = new List<BlogResult>();

    public ICollection<InvitationResult> Invitations { get; set; } = new List<InvitationResult>();

    public ICollection<ReviewResult> Reviews { get; set; } = new List<ReviewResult>();

    public ICollection<TeamMemberResult> TeamMembers { get; set; } = new List<TeamMemberResult>();
}