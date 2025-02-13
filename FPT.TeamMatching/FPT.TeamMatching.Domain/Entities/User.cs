using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class User : BaseEntity
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
    
    public virtual ICollection<IdeaReview> IdeaReviews { get; set; } = new List<IdeaReview>();

    public virtual ICollection<Idea> Ideas { get; set; } = new List<Idea>();
    
    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<InvitationUser> InvitationUserSenders { get; set; } = new List<InvitationUser>();

    public virtual ICollection<InvitationUser> InvitationUserReceivers { get; set; } = new List<InvitationUser>();

    public virtual ICollection<JobPosition> JobPositions { get; set; } = new List<JobPosition>();

    public virtual ICollection<LecturerFeedback> LecturerFeedbacks { get; set; } = new List<LecturerFeedback>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Profile? Profile { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Rate> RateBys { get; set; } = new List<Rate>();

    public virtual ICollection<Rate> RateFors { get; set; } = new List<Rate>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<SkillProfile> SkillProfiles { get; set; } = new List<SkillProfile>();

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual VerifyQualifiedForAcademicProject? VerifyQualifiedForAcademicProject { get; set; }

    public virtual ICollection<VerifyQualifiedForAcademicProject> VerifyQualifiedForAcademicProjects { get; set; } =
        new List<VerifyQualifiedForAcademicProject>();

    public virtual VerifySemester? VerifySemester { get; set; }

    public virtual ICollection<VerifySemester> VerifySemesters { get; set; } = new List<VerifySemester>();
}