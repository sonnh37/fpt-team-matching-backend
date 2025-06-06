﻿using FPT.TeamMatching.Domain.Entities.Base;
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

    public string? Code { get; set; }

    public string? Email { get; set; }

    public DateTimeOffset? Dob { get; set; }

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Department? Department { get; set; }

    public virtual ProfileStudent? ProfileStudent { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<TopicRequest> TopicRequestOfReviewers { get; set; } = new List<TopicRequest>();

    public virtual ICollection<Topic> TopicOfOwners { get; set; } = new List<Topic>();

    public virtual ICollection<Topic> TopicOfMentors { get; set; } = new List<Topic>();

    public virtual ICollection<Topic> TopicOfSubMentors { get; set; } = new List<Topic>();

    public virtual ICollection<TopicVersionRequest> TopicVersionRequestOfReviewers { get; set; } = new List<TopicVersionRequest>();

    public virtual ICollection<UserXRole> UserXRoles { get; set; } = new List<UserXRole>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Invitation> InvitationOfSenders { get; set; } = new List<Invitation>();

    public virtual ICollection<Invitation> InvitationOfReceivers { get; set; } = new List<Invitation>();

    public virtual ICollection<BlogCv> BlogCvs { get; set; } = new List<BlogCv>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<SkillProfile> SkillProfiles { get; set; } = new List<SkillProfile>();

    public virtual ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();

    public virtual ICollection<Review> Reviewer1s { get; set; } = new List<Review>();

    public virtual ICollection<Review> Reviewer2s { get; set; } = new List<Review>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<NotificationXUser> NotificationXUsers { get; set; } = new List<NotificationXUser>();
}