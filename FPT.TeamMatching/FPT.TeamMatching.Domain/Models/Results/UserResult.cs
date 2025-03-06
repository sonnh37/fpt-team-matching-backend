using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Models.Results;

public class UserResult : BaseResult
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    public string? Avatar { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Department? Department { get; set; }

    public ICollection<BlogResult> Blogs { get; set; } = new List<BlogResult>();

    public ICollection<IdeaRequestResult> IdeaRequestOfReviewers { get; set; } = new List<IdeaRequestResult>();

    public ICollection<IdeaHistoryRequestResult> IdeaHistoryRequestOfReviewers { get; set; } =
        new List<IdeaHistoryRequestResult>();

    public ICollection<IdeaResult> IdeaOfOwners { get; set; } = new List<IdeaResult>();

    public ICollection<IdeaResult> IdeaOfMentors { get; set; } = new List<IdeaResult>();

    public ICollection<IdeaResult> IdeaOfSubMentors { get; set; } = new List<IdeaResult>();

    public ICollection<IdeaHistoryResult> IdeaHistoryOfCouncils { get; set; } = new List<IdeaHistoryResult>();

    public ICollection<UserXRoleResult> UserXRoles { get; set; } = new List<UserXRoleResult>();

    public ICollection<CommentResult> Comments { get; set; } = new List<CommentResult>();

    public ICollection<InvitationResult> InvitationOfSenders { get; set; } = new List<InvitationResult>();

    public ICollection<InvitationResult> InvitationOfReceivers { get; set; } = new List<InvitationResult>();

    public ICollection<BlogCvResult> BlogCvs { get; set; } = new List<BlogCvResult>();

    public ICollection<LikeResult> Likes { get; set; } = new List<LikeResult>();

    public ICollection<NotificationResult> Notifications { get; set; } = new List<NotificationResult>();

    public ProfileStudentResult? ProfileStudent { get; set; }

    public ICollection<ProjectResult> Projects { get; set; } = new List<ProjectResult>();

    public ICollection<ProjectResult> ProjectOfLeaders { get; set; } = new List<ProjectResult>();

    public ICollection<RefreshTokenResult> RefreshTokens { get; set; } = new List<RefreshTokenResult>();

    public ICollection<SkillProfileResult> SkillProfiles { get; set; } = new List<SkillProfileResult>();

    public ICollection<TeamMemberResult> TeamMembers { get; set; } = new List<TeamMemberResult>();
}