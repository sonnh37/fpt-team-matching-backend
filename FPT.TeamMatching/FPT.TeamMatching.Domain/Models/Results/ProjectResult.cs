using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class ProjectResult : BaseResult
{
    public Guid? LeaderId { get; set; }

    public Guid? SemesterId { get; set; }

    public Guid? TopicId { get; set; }

    public string? TeamCode { get; set; }

    public string? TeamName { get; set; }

    public ProjectStatus? Status { get; set; }

    public int? TeamSize { get; set; }

    public int? DefenseStage { get; set; }

    public virtual UserResult? Leader { get; set; }

    public virtual TopicResult? Topic { get; set; }

    public virtual SemesterResult? Semester { get; set; }

    public virtual MentorFeedbackResult? MentorFeedback { get; set; }

    public virtual ICollection<InvitationResult> Invitations { get; set; } = new List<InvitationResult>();

    public virtual ICollection<ReviewResult> Reviews { get; set; } = new List<ReviewResult>();

    public virtual ICollection<TeamMemberResult> TeamMembers { get; set; } = new List<TeamMemberResult>();

    public virtual ICollection<BlogResult> Blogs { get; set; } = new List<BlogResult>();

    public virtual ICollection<MentorTopicRequestResult> MentorTopicRequests { get; set; } = new List<MentorTopicRequestResult>();

    public virtual ICollection<CapstoneScheduleResult> CapstoneSchedules { get; set; } = new List<CapstoneScheduleResult>();

    public virtual ICollection<NotificationResult> Notifications { get; set; } = new List<NotificationResult>();
}