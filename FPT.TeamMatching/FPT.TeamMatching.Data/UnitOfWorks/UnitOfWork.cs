using FPT.TeamMatching.Data.Context;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

namespace FPT.TeamMatching.Data.UnitOfWorks;

public class UnitOfWork : BaseUnitOfWork<FPTMatchingDbContext>, IUnitOfWork
{
    public UnitOfWork(FPTMatchingDbContext context, IServiceProvider serviceProvider) : base(context, serviceProvider)
    {
    }

    public IUserRepository UserRepository => GetRepository<IUserRepository>();
    public IRoleRepository RoleRepository => GetRepository<IRoleRepository>();
    public IUserXRoleRepository UserXRoleRepository => GetRepository<IUserXRoleRepository>();
    public IRefreshTokenRepository RefreshTokenRepository => GetRepository<IRefreshTokenRepository>();

    public IBlogRepository BlogRepository => GetRepository<IBlogRepository>();
    public ILikeRepository LikeRepository => GetRepository<ILikeRepository>();
    public ICommentRepository CommentRepository => GetRepository<ICommentRepository>();
    public IBlogCvRepository BlogCvRepository => GetRepository<IBlogCvRepository>();

    public IRateRepository RateRepository => GetRepository<IRateRepository>();
    public ITeamMemberRepository TeamMemberRepository => GetRepository<ITeamMemberRepository>();
    public IInvitationRepository InvitationRepository => GetRepository<IInvitationRepository>();
    public IProjectRepository ProjectRepository => GetRepository<IProjectRepository>();
    public IReviewRepository ReviewRepository => GetRepository<IReviewRepository>();

    public INotificationRepository NotificationRepository => GetRepository<INotificationRepository>();
    public IProfileStudentRepository ProfileStudentRepository => GetRepository<IProfileStudentRepository>();
    public ISkillProfileRepository SkillProfileRepository => GetRepository<ISkillProfileRepository>();
    
    public IProfessionRepository ProfessionRepository => GetRepository<IProfessionRepository>();
    public ISpecialtyRepository SpecialtyRepository => GetRepository<ISpecialtyRepository>();
    public ISemesterRepository SemesterRepository => GetRepository<ISemesterRepository>();
    public IStageTopicRepository StageTopicRepository => GetRepository<IStageTopicRepository>();
    public IMentorTopicRequestRepository MentorTopicRequestRepository => GetRepository<IMentorTopicRequestRepository>();
    public ICapstoneScheduleRepository CapstoneScheduleRepository => GetRepository<ICapstoneScheduleRepository>();
    public ICriteriaRepository CriteriaRepository => GetRepository<ICriteriaRepository>();
    public ICriteriaFormRepository CriteriaFormRepository => GetRepository<ICriteriaFormRepository>();
    public ICriteriaXCriteriaFormRepository CriteriaXCriteriaFormRepository => GetRepository<ICriteriaXCriteriaFormRepository>();
    public IAnswerCriteriaRepository AnswerCriteriaRepository => GetRepository<IAnswerCriteriaRepository>();

    public ITopicRepository TopicRepository => GetRepository<ITopicRepository>();
    public ITopicVersionRepository TopicVersionRepository => GetRepository<ITopicVersionRepository>();
    public ITopicVersionRequestRepository TopicVersionRequestRepository => GetRepository<ITopicVersionRequestRepository>();
    public ITopicRequestRepository TopicRequestRepository => GetRepository<ITopicRequestRepository>();

    public IMentorFeedbackRepository MentorFeedbackRepository => GetRepository<IMentorFeedbackRepository>();
    public INotificationXUserRepository NotificationXUserRepository => GetRepository<INotificationXUserRepository>();
}