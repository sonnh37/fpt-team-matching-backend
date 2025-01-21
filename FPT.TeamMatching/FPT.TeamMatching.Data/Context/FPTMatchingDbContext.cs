using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Action = FPT.TeamMatching.Domain.Entities.Action;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Data.Context;

public partial class FPTMatchingDbContext : BaseDbContext
{
    public FPTMatchingDbContext(DbContextOptions<FPTMatchingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Action> Actions { get; set; }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<InvitationUser> InvitationUsers { get; set; }

    public virtual DbSet<JobPosition> JobPositions { get; set; }

    public virtual DbSet<LecturerFeedback> LecturerFeedbacks { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionXAction> PermissionXactions { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectActivity> ProjectActivities { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<SkillProfile> SkillProfiles { get; set; }

    public virtual DbSet<Task> Tasks { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserXPermission> UserXpermissions { get; set; }

    public virtual DbSet<VerifyQualifiedForAcademicProject> VerifyQualifiedForAcademicProjects { get; set; }

    public virtual DbSet<VerifySemester> VerifySemesters { get; set; }

    // Auto Enum Convert Int To String
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
 
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Action>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Action");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Blog");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId);
        });

        // modelBuilder.Entity<BlogType>(entity =>
        // {
        //     entity.HasKey(e => e.Id);
        //
        //     entity.ToTable("BlogType");
        //
        //     entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        // });

        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Chat");
            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Sender).WithMany(p => p.ChatSenders)
                .HasForeignKey(d => d.SenderId);

            entity.HasOne(d => d.Receiver).WithMany(p => p.ChatReceivers)
                .HasForeignKey(d => d.ReceiverId);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Comment");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Blog).WithMany(p => p.Comments)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<InvitationUser>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("InvitationUser");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Project).WithMany(p => p.InvitationUsers)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.Sender).WithMany(p => p.InvitationUserSenders)
                .HasForeignKey(d => d.SenderId);

            entity.HasOne(d => d.Receiver).WithMany(p => p.InvitationUserReceivers)
                .HasForeignKey(d => d.ReceiverId);
        });

        modelBuilder.Entity<JobPosition>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("JobPosition");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.FileCv).HasMaxLength(1);

            entity.HasOne(d => d.Blog).WithMany(p => p.JobPositions)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.JobPositions)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<LecturerFeedback>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("LecturerFeedback");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Lecturer).WithMany(p => p.LecturerFeedbacks)
                .HasForeignKey(d => d.LecturerId);

            entity.HasOne(d => d.Report).WithMany(p => p.LecturerFeedbacks)
                .HasForeignKey(d => d.ReportId);
        });

        modelBuilder.Entity<Like>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Like");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Blog).WithMany(p => p.Likes)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.Likes)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Notification");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Permission");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<PermissionXAction>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("PermissionXAction");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Action).WithMany()
                .HasForeignKey(d => d.ActionId);

            entity.HasOne(d => d.Permission).WithMany()
                .HasForeignKey(d => d.PermissionId);
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Profile");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithOne(p => p.Profile)
                .HasForeignKey<Profile>(d => d.UserId);
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Project");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Leader).WithMany(p => p.Projects)
                .HasForeignKey(d => d.LeaderId);
        });

        modelBuilder.Entity<ProjectActivity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("ProjectActivity");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Project).WithMany(p => p.ProjectActivities)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Rate");

            entity.HasOne(d => d.RateBy).WithMany(p => p.RateBys)
                .HasForeignKey(d => d.RateById);

            entity.HasOne(d => d.RateFor).WithMany(p => p.RateFors)
                .HasForeignKey(d => d.RateForId);

            entity.HasOne(d => d.TeamMember).WithMany(p => p.Rates)
                .HasForeignKey(d => d.TeamMemberId);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("RefreshToken");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Report");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Project).WithMany(p => p.Reports)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<SkillProfile>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("SkillProfile");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithMany(p => p.SkillProfiles)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Task>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Task");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.AssignedTo).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.AssignedToId);

            entity.HasOne(d => d.Project).WithMany(p => p.Tasks)
                .HasForeignKey(d => d.ProjectId);
        });

        modelBuilder.Entity<TeamMember>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("TeamMember");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Project).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.ProjectId);

            entity.HasOne(d => d.User).WithMany(p => p.TeamMembers)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("User");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<UserXPermission>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("UserXPermission");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.Permission).WithMany()
                .HasForeignKey(d => d.PermissionId);

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<VerifyQualifiedForAcademicProject>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("VerifyQualifiedForAcademicProject");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithOne(p => p.VerifyQualifiedForAcademicProject)
                .HasForeignKey<VerifyQualifiedForAcademicProject>(d => d.UserId);

            entity.HasOne(d => d.VerifyBy).WithMany(p => p.VerifyQualifiedForAcademicProjects)
                .HasForeignKey(d => d.VerifyById);
        });

        modelBuilder.Entity<VerifySemester>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("VerifySemester");

            entity.HasIndex(e => e.UserId).IsUnique();

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");

            entity.HasOne(d => d.User).WithOne(p => p.VerifySemester)
                .HasForeignKey<VerifySemester>(d => d.UserId);

            entity.HasOne(d => d.VerifyBy).WithMany(p => p.VerifySemesters)
                .HasForeignKey(d => d.VerifyById);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    #region Config

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlServer(GetConnectionString());
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