using FPT.TeamMatching.API.Hubs;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Services;
using Microsoft.AspNetCore.SignalR;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionServices
{
    public static void AddCollectionServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IUserXRoleService, UserXRoleService>();
        services.AddTransient<IBlogService, BlogService>();
        services.AddTransient<ILikeService, LikeService>();
        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IRateService, RateService>();
        services.AddTransient<IBlogCvService, BlogCvService>();
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
        services.AddTransient<IProfessionService, ProfessionService>();
        services.AddTransient<ISpecialtyService, SpecialtyService>();
        services.AddTransient<ISemesterService, SemesterService>();
        services.AddTransient<IIdeaRequestService, IdeaRequestService>();
        services.AddTransient<IIdeaService, IdeaService>();
        services.AddTransient<ITopicVersionService, TopicVersionService>();
        services.AddTransient<ITopicService, TopicService>();
        services.AddTransient<IStageIdeaService, StageIdeaService>();
        services.AddTransient<ICapstoneScheduleService, CapstoneScheduleService>();
        services.AddTransient<IMentorIdeaRequestService, MentorIdeaRequestService>();
        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
        services.AddSingleton<IFileUploadService, FileUploadService>();
        services.AddTransient<IMentorFeedbackService, MentorFeedbackService>();
        services.AddTransient<ITimelineService, TimelineService>();
        services.AddTransient<ICriteriaService, CriteriaService>();
        services.AddTransient<ICriteriaFormService, CriteriaFormService>();
        services.AddTransient<ICriteriaXCriteriaFormService, CriteriaXCriteriaFormService>();
        services.AddTransient<IAnswerCriteriaService, AnswerCriteriaService>();

        services.AddTransient<IApiHubService, ApiHubService>();
        services.AddTransient<ChatHub>();
    }
}