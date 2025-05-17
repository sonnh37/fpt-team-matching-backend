using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class UserResult : BaseResult
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    // public string? Password { get; set; }

    public string? Avatar { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Code { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Department? Department { get; set; }

    public virtual ProfileStudentResult? ProfileStudent { get; set; }

    public virtual ICollection<BlogResult> Blogs { get; set; } = new List<BlogResult>();

    public virtual ICollection<TopicRequestResult> TopicRequestOfReviewers { get; set; } = new List<TopicRequestResult>();

    public virtual ICollection<TopicResult> TopicOfOwners { get; set; } = new List<TopicResult>();

    public virtual ICollection<TopicResult> TopicOfMentors { get; set; } = new List<TopicResult>();

    public virtual ICollection<TopicResult> TopicOfSubMentors { get; set; } = new List<TopicResult>();

    public virtual ICollection<TopicVersionRequestResult> TopicVersionRequestOfReviewers { get; set; } = new List<TopicVersionRequestResult>();

    public virtual ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();

    public virtual ICollection<CommentResult> Comments { get; set; } = new List<CommentResult>();

    public virtual ICollection<InvitationResult> InvitationOfSenders { get; set; } = new List<InvitationResult>();

    public virtual ICollection<InvitationResult> InvitationOfReceivers { get; set; } = new List<InvitationResult>();

    public virtual ICollection<BlogCvResult> BlogCvs { get; set; } = new List<BlogCvResult>();

    public virtual ICollection<LikeResult> Likes { get; set; } = new List<LikeResult>();

    public virtual ICollection<ProjectResult> Projects { get; set; } = new List<ProjectResult>();

    public virtual ICollection<RefreshTokenResult> RefreshTokens { get; set; } = new List<RefreshTokenResult>();

    public virtual ICollection<SkillProfileResult> SkillProfiles { get; set; } = new List<SkillProfileResult>();

    public virtual ICollection<TeamMemberResult> TeamMembers { get; set; } = new List<TeamMemberResult>();

    public virtual ICollection<ReviewResult> Reviewer1s { get; set; } = new List<ReviewResult>();

    public virtual ICollection<ReviewResult> Reviewer2s { get; set; } = new List<ReviewResult>();

    public virtual ICollection<NotificationResult> Notifications { get; set; } = new List<NotificationResult>();
}