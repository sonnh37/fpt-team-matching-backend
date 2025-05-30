﻿using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FPT.TeamMatching.Data.Context;

public partial class FPTMatchingDbContext : BaseDbContext
{
    public FPTMatchingDbContext(DbContextOptions<FPTMatchingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; } //

    public virtual DbSet<Comment> Comments { get; set; } //

    public virtual DbSet<Invitation> Invitations { get; set; } //

    public virtual DbSet<BlogCv> BlogCvs { get; set; } //

    public virtual DbSet<Like> Likes { get; set; } //

    public virtual DbSet<Notification> Notifications { get; set; } //

    public virtual DbSet<NotificationXUser> NotificationXUsers { get; set; } //

    public virtual DbSet<Profession> Professions { get; set; } //

    public virtual DbSet<Specialty> Specialties { get; set; } //

    public virtual DbSet<ProfileStudent> ProfileStudents { get; set; } //

    public virtual DbSet<Project> Projects { get; set; } //

    public virtual DbSet<Rate> Rates { get; set; } //

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; } //

    public virtual DbSet<Review> Reviews { get; set; } //

    public virtual DbSet<SkillProfile> SkillProfiles { get; set; } //

    public virtual DbSet<TeamMember> TeamMembers { get; set; } //

    public virtual DbSet<User> Users { get; set; } //

    public virtual DbSet<Semester> Semesters { get; set; } //

    public virtual DbSet<UserXRole> UserXRoles { get; set; } //

    public virtual DbSet<Role> Roles { get; set; } //

    public virtual DbSet<Topic> Topics { get; set; } //

    public virtual DbSet<TopicRequest> TopicRequests { get; set; } //

    public virtual DbSet<TopicVersion> TopicVersions { get; set; } //

    public virtual DbSet<TopicVersionRequest> TopicVersionRequests { get; set; } //

    public virtual DbSet<CapstoneSchedule> CapstoneSchedules { get; set; } //

    public virtual DbSet<StageTopic> StageTopics { get; set; } //

    public virtual DbSet<MentorTopicRequest> MentorTopicRequests { get; set; } //

    public virtual DbSet<MentorFeedback> MentorFeedbacks { get; set; } //

    public virtual DbSet<Criteria> Criterias { get; set; } //

    public virtual DbSet<CriteriaForm> CriteriaForms { get; set; } //

    public virtual DbSet<CriteriaXCriteriaForm> CriteriaXCriteriaForms { get; set; } //

    public virtual DbSet<AnswerCriteria> AnswerCriterias { get; set; } //


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Blog");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Project).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Comment");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Invitation");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithMany(p => p.Invitations)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.Sender).WithMany(p => p.InvitationOfSenders)
                .HasForeignKey(d => d.SenderId);

            entity.HasOne(d => d.Receiver).WithMany(p => p.InvitationOfReceivers)
                .HasForeignKey(d => d.ReceiverId);
        });

        modelBuilder.Entity<BlogCv>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("BlogCv");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogCvs)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.BlogCvs)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Like");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Blog).WithMany(p => p.Likes)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Notification");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Project).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<NotificationXUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("NotificationXUser");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.NotificationXUsers)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Notification).WithMany(p => p.NotificationXUsers)
                .HasForeignKey(d => d.NotificationId);
        });

        modelBuilder.Entity<ProfileStudent>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("ProfileStudent");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithOne(p => p.ProfileStudent)
                .HasForeignKey<ProfileStudent>(d => d.UserId);

            entity.HasOne(d => d.Specialty).WithMany(p => p.ProfileStudents)
                .HasForeignKey(d => d.SpecialtyId);

            entity.HasOne(d => d.Semester).WithMany(p => p.ProfileStudents)
                .HasForeignKey(d => d.SemesterId);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Project");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Leader).WithMany(p => p.Projects)
                .HasForeignKey(d => d.LeaderId);

            entity.HasOne(d => d.Topic).WithOne(p => p.Project)
                .HasForeignKey<Project>(d => d.TopicId);

            entity.HasMany(p => p.TeamMembers)
                .WithOne(t => t.Project)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Invitations)
                .WithOne(i => i.Project)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Reviews)
                .WithOne(r => r.Project)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.Blogs)
                .WithOne(b => b.Project)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.CapstoneSchedules)
                .WithOne(c => c.Project)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(p => p.MentorTopicRequests)
                .WithOne(m => m.Project)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Rate");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.RateBy).WithMany(p => p.RateBys)
                .HasForeignKey(d => d.RateById);

            entity.HasOne(d => d.RateFor).WithMany(p => p.RateFors)
                .HasForeignKey(d => d.RateForId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Review");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.Reviewer1).WithMany(p => p.Reviewer1s)
                .HasForeignKey(d => d.Reviewer1Id);

            entity.HasOne(d => d.Reviewer2).WithMany(p => p.Reviewer2s)
                .HasForeignKey(d => d.Reviewer2Id);
        });

        modelBuilder.Entity<SkillProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("SkillProfile");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.ProfileStudent).WithMany(p => p.SkillProfiles)
                .HasForeignKey(d => d.ProfileStudentId);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TeamMember");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Role");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("User");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<UserXRole>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("UserXRole");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.UserXRoles)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.Role).WithMany(p => p.UserXRoles)
                .HasForeignKey(d => d.RoleId);

            entity.HasOne(d => d.Semester).WithMany(p => p.UserXRoles)
                .HasForeignKey(d => d.SemesterId);
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Semester");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.CriteriaForm).WithMany(p => p.Semesters)
                .HasForeignKey(d => d.CriteriaFormId);
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Topic");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Owner).WithMany(p => p.TopicOfOwners)
                .HasForeignKey(d => d.OwnerId);

            entity.HasOne(d => d.Mentor).WithMany(p => p.TopicOfMentors)
                .HasForeignKey(d => d.MentorId);

            entity.HasOne(d => d.SubMentor).WithMany(p => p.TopicOfSubMentors)
                .HasForeignKey(d => d.SubMentorId);

            entity.HasOne(d => d.Specialty).WithMany(p => p.Topics)
                .HasForeignKey(d => d.SpecialtyId);

            entity.HasOne(d => d.StageTopic).WithMany(p => p.Topics)
                .HasForeignKey(d => d.StageTopicId);

            entity.HasOne(d => d.Semester).WithMany(p => p.Topics)
                .HasForeignKey(d => d.SemesterId);
        });

        modelBuilder.Entity<TopicRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TopicRequest");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Topic).WithMany(p => p.TopicRequests)
                .HasForeignKey(d => d.TopicId).OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.TopicRequestOfReviewers)
                .HasForeignKey(d => d.ReviewerId);

            entity.HasOne(d => d.CriteriaForm).WithMany(p => p.TopicRequests)
                .HasForeignKey(d => d.CriteriaFormId);
        });

        modelBuilder.Entity<TopicVersion>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TopicVersion");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Topic).WithMany(p => p.TopicVersions)
                .HasForeignKey(d => d.TopicId);
        });

        modelBuilder.Entity<TopicVersionRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TopicVersionRequest");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.TopicVersion).WithMany(p => p.TopicVersionRequests)
                .HasForeignKey(d => d.TopicVersionId);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.TopicVersionRequestOfReviewers)
                .HasForeignKey(d => d.ReviewerId);
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Specialty");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Profession).WithMany(p => p.Specialties)
                .HasForeignKey(d => d.ProfessionId);
        });

        modelBuilder.Entity<Profession>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Profession");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<CapstoneSchedule>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("CapstoneSchedule");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithMany(p => p.CapstoneSchedules)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<StageTopic>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("StageTopic");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Semester).WithMany(p => p.StageTopics)
                .HasForeignKey(d => d.SemesterId);
        });

        modelBuilder.Entity<MentorTopicRequest>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("MentorTopicRequest");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithMany(p => p.MentorTopicRequests)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.Topic).WithMany(p => p.MentorTopicRequests)
                .HasForeignKey(d => d.TopicId);
        });

        modelBuilder.Entity<MentorFeedback>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("MentorFeedback");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Project).WithOne(p => p.MentorFeedback)
                .HasForeignKey<MentorFeedback>(d => d.ProjectId);
        });

        //modelBuilder.Entity<Timeline>(entity =>
        //{
        //    entity.HasKey(e => e.Id);

        //    entity.ToTable("Timeline");

        //    entity.Property(e => e.Id).ValueGeneratedOnAdd()
        //        .HasDefaultValueSql("gen_random_uuid()");

        //    entity.HasOne(d => d.Semester).WithMany(p => p.Timelines)
        //        .HasForeignKey(d => d.SemesterId);
        //});

        modelBuilder.Entity<Criteria>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Criteria");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<CriteriaForm>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("CriteriaForm");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<CriteriaXCriteriaForm>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("CriteriaXCriteriaForm");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Criteria).WithMany(p => p.CriteriaXCriteriaForms)
                .HasForeignKey(d => d.CriteriaId);

            entity.HasOne(d => d.CriteriaForm).WithMany(p => p.CriteriaXCriteriaForms)
                .HasForeignKey(d => d.CriteriaFormId);
        });

        modelBuilder.Entity<AnswerCriteria>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("AnswerCriteria");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.TopicRequest).WithMany(p => p.AnswerCriterias)
                .HasForeignKey(d => d.TopicRequestId);

            entity.HasOne(d => d.Criteria).WithMany(p => p.AnswerCriterias)
                .HasForeignKey(d => d.CriteriaId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    #region Config

    // Auto Enum Convert Int To String
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseNpgsql(GetConnectionString());
    }

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
        var strConn = config.GetConnectionString("DefaultConnection");

        return strConn;
    }

    #endregion
}