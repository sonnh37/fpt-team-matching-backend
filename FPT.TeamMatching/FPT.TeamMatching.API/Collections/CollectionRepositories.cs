﻿using FPT.TeamMatching.Data.Repositories;
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
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        
        services.AddScoped<IInvitationRepository, InvitationRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IRateRepository, RateRepository>();
        services.AddScoped<IBlogCvRepository, BlogCvRepository>();
        services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IProfileStudentRepository, ProfileStudentRepository>();
        services.AddScoped<ISkillProfileRepository, SkillProfileRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IConversationMemberRepository, ConversationMemberRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserXRoleRepository, UserXRoleRepository>();

        services.AddScoped<IProfessionRepository, ProfessionRepository>();
        services.AddScoped<ISpecialtyRepository, SpecialtyRepository>();

        services.AddScoped<ISemesterRepository, SemesterRepository>();
        services.AddScoped<IStageTopicRepository, StageTopicRepository>();
        services.AddScoped<ICapstoneScheduleRepository, CapstoneScheduleRepository>();
        services.AddScoped<IMentorTopicRequestRepository, MentorTopicRequestRepository>();
        services.AddScoped<IMentorFeedbackRepository, MentorFeedbackRepository>();

        services.AddScoped<ITopicRepository, TopicRepository>();
        services.AddScoped<ITopicRequestRepository, TopicRequestRepository>();

        services.AddScoped<ITopicVersionRepository, TopicVersionRepository>();
        services.AddScoped<ITopicVersionRequestRepository, TopicVersionRequestRepository>();

        services.AddScoped<ICriteriaRepository, CriteriaRepository>();
        services.AddScoped<ICriteriaFormRepository, CriteriaFormRepository>();
        services.AddScoped<ICriteriaXCriteriaFormRepository, CriteriaXCriteriaFormRepository>();
        services.AddScoped<IAnswerCriteriaRepository, AnswerCriteriaRepository>();
        services.AddScoped<INotificationXUserRepository, NotificationXUserRepository>();
    }
}