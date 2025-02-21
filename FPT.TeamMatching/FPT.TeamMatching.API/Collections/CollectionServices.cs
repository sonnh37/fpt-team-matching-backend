﻿using FPT.TeamMatching.API.Hub;
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
        services.AddTransient<ISpecialtyService, RateService>();
        services.AddTransient<IBlogCvService, BlogCvService>();
        services.AddTransient<ITeamMemberService, TeamMemberService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IReviewService, ReviewService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IFeedbackService, FeedbackService>();
        services.AddTransient<IInvitationService, InvitationService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
        services.AddTransient<INotificationService, NotificationService>();
        services.AddTransient<IProfileStudentService, ProfileStudentService>();
        services.AddTransient<ISkillProfileService, SkillProfileService>();
        services.AddTransient<IMessageService, MessageService>();
        services.AddTransient<IConversationMemberService, ConversationMemberService>();
        services.AddTransient<IProfessionService, ProfessionService>();
        services.AddTransient<ISpecialtyService, SpecialtyService>();
        services.AddTransient<IIdeaReviewService, IdeaReviewService>();
        services.AddTransient<IIdeaService, IdeaService>();

        services.AddTransient<ChatHub>();
    }
}