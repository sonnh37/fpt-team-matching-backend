using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ProjectResult : BaseResult
{
    public Guid? LeaderId { get; set; }

    public Guid? IdeaId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public int? DefenseStage { get; set; }

    public IdeaResult? Idea { get; set; }

    public ICollection<TeamMemberResult> TeamMembers { get; set; } = new List<TeamMemberResult>();

    public ICollection<InvitationResult> Invitations { get; set; } = new List<InvitationResult>();

    public ICollection<ReviewResult> Reviews { get; set; } = new List<ReviewResult>();
}