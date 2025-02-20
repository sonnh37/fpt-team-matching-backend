using FPT.TeamMatching.Domain.Entities.Base;
using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Entities;

public class User : BaseEntity
{
    public Gender? Gender { get; set; }

    public string? Cache { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Avatar { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Department? Department { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<IdeaReview> IdeaReviews { get; set; } = new List<IdeaReview>();

    public virtual ICollection<Idea> IdeaOfUsers { get; set; } = new List<Idea>();

    public virtual ICollection<Idea> IdeaOfSubMentors { get; set; } = new List<Idea>();

    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Invitation> InvitationOfSenders { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationOfReceivers { get; set; } = new List<Invitation>();

    public virtual ICollection<BlogCv> BlogCvs { get; set; } = new List<BlogCv>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ProfileStudent? ProfileStudent { get; set; }

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<Project> ProjectOfLeaders { get; set; } = new List<Project>();

    public virtual ICollection<Rate> RateBys { get; set; } = new List<Rate>();

    public virtual ICollection<Rate> RateFors { get; set; } = new List<Rate>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<SkillProfile> SkillProfiles { get; set; } = new List<SkillProfile>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
}