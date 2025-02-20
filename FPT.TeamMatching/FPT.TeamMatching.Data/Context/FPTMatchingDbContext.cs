using FPT.TeamMatching.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FPT.TeamMatching.Data.Context;

public partial class FPTMatchingDbContext : BaseDbContext
{
    public FPTMatchingDbContext(DbContextOptions<FPTMatchingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<BlogCv> BlogCvs { get; set; }

    public virtual DbSet<Like> Likes { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<ProfileStudent> Profiles { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<SkillProfile> SkillProfiles { get; set; }

    public virtual DbSet<TeamMember> TeamMembers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Semester> Semesters { get; set; }

    public virtual DbSet<Idea> Ideas { get; set; }

    public virtual DbSet<IdeaReview> IdeaReviews { get; set; }

    public virtual DbSet<UserXRole> UserXRoles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Ignore<Task>(); 
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Blog");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.UserId);
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
            entity.Property(e => e.FileCv).HasMaxLength(1);

            entity.HasOne(d => d.Blog).WithMany(p => p.BlogCvs)
                .HasForeignKey(d => d.BlogId);

            entity.HasOne(d => d.User).WithMany(p => p.BlogCvs)
                .HasForeignKey(d => d.UserId);
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Feedback");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Review).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ReviewId);
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
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Project");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Leader).WithMany(p => p.Projects)
                .HasForeignKey(d => d.LeaderId);

            entity.HasOne(d => d.Idea).WithOne(p => p.Project)
                .HasForeignKey<Project>(d => d.IdeaId);
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

            entity.HasOne(d => d.TeamMember).WithMany(p => p.Rates)
                .HasForeignKey(d => d.TeamMemberId);
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
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Semester");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");
        });

        modelBuilder.Entity<Idea>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Idea");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.User).WithMany(p => p.IdeaOfUsers)
                .HasForeignKey(d => d.UserId);

            entity.HasOne(d => d.SubMentor).WithMany(p => p.IdeaOfSubMentors)
                .HasForeignKey(d => d.SubMentorId);

            entity.HasOne(d => d.Semester).WithMany(p => p.Ideas)
                .HasForeignKey(d => d.SemesterId);
            
            entity.HasOne(d => d.Specialty).WithMany(p => p.Ideas)
                .HasForeignKey(d => d.SpecialtyId);
        });

        modelBuilder.Entity<IdeaReview>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("IdeaReview");

            entity.Property(e => e.Id).ValueGeneratedOnAdd()
                .HasDefaultValueSql("gen_random_uuid()");

            entity.HasOne(d => d.Idea).WithMany(p => p.IdeaReviews)
                .HasForeignKey(d => d.IdeaId);

            entity.HasOne(d => d.Reviewer).WithMany(p => p.IdeaReviews)
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