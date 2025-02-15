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
        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<ITeamMemberService, TeamMemberService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IReviewService, ReviewService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IInvitationService, InvitationService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IProfileStudentService, ProfileStudentService>();
        services.AddTransient<ISkillProfileService, SkillProfileService>();
        services.AddTransient<IMessageService, MessageService>();
        services.AddTransient<IConversationMemberService, ConversationMemberService>();

        services.AddTransient<ChatHub>();
    }
}