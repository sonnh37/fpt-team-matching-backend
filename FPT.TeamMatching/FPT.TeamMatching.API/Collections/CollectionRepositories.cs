using FPT.TeamMatching.Data.Repositories;
using FPT.TeamMatching.Data.UnitOfWorks;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionRepositories
{
    public static void AddCollectionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMongoUnitOfWork, MongoUnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectActivityRepository, ProjectActivityRepository>();
        services.AddScoped<ILecturerFeedbackRepository, LecturerFeedbackRepository>();
        services.AddScoped<IInvitationUserRepository, InvitationUserRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRateRepository, RateRepository>();
        services.AddScoped<IJobPositionRepository, JobPositionRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        services.AddScoped<ISkillProfileRepository, SkillProfileRepository>();
        services.AddScoped<IVerifySemesterRepository, VerifySemesterRepository>();
        services.AddScoped<IVerifyQualifiedForAcademicProjectRepository, VerifyQualifiedForAcademicProjectRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IConversationMemberRepository, ConversationMemberRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserXRoleRepository, UserXRoleRepository>();
    }
}