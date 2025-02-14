using FPT.TeamMatching.API.Hub;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Services;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionServices
{
    public static void AddCollectionServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IBlogService, BlogService>();
        services.AddTransient<ILikeService, LikeService>();
        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IRateService, RateService>();
        services.AddTransient<IJobPositionService, JobPositionService>();
        services.AddTransient<ITeamMemberService, TeamMemberService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ITaskService, IdeaReviewService>();
        services.AddTransient<IReviewService, ReviewService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IUserXProjectService, IdeaService>();
        services.AddTransient<IFeedbackService, FeedbackService>();
        services.AddTransient<IInvitationService, InvitationService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IProfileService, ProfileService>();
        services.AddTransient<ISkillProfileService, SkillProfileService>();
        services.AddTransient<IVerifyQualifiedForAcademicProjectService, VerifyQualifiedForAcademicProjectService>();
        services.AddTransient<IVerifySemesterService, VerifySemesterService>();
        
        services.AddTransient<IMessageService, MessageService>();

        services.AddTransient<ChatHub>();
    }
}