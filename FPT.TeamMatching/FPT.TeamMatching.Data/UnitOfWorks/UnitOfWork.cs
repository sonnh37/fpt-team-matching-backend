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
    public IBlogRepository BlogRepository => GetRepository<IBlogRepository>();
    public ILikeRepository LikeRepository => GetRepository<ILikeRepository>();
    public ICommentRepository CommentRepository => GetRepository<ICommentRepository>();
    public IRateRepository RateRepository => GetRepository<IRateRepository>();
    public IIdeaRepository IdeaRepository => GetRepository<IIdeaRepository>();
    public IIdeaVersionRequestRepository IdeaVersionRequestRepository => GetRepository<IIdeaVersionRequestRepository>();
    public IBlogCvRepository BlogCvRepository => GetRepository<IBlogCvRepository>();
    public ITeamMemberRepository TeamMemberRepository => GetRepository<ITeamMemberRepository>();
    public IReviewRepository ReviewRepository => GetRepository<IReviewRepository>();
    public IInvitationRepository InvitationRepository => GetRepository<IInvitationRepository>();
    public IRefreshTokenRepository RefreshTokenRepository => GetRepository<IRefreshTokenRepository>();
    public IProjectRepository ProjectRepository => GetRepository<IProjectRepository>();
    public INotificationRepository NotificationRepository => GetRepository<INotificationRepository>();
    public IProfileStudentRepository ProfileStudentRepository => GetRepository<IProfileStudentRepository>();
    public ISkillProfileRepository SkillProfileRepository => GetRepository<ISkillProfileRepository>();
    public IRoleRepository RoleRepository => GetRepository<IRoleRepository>();
    public IUserXRoleRepository UserXRoleRepository => GetRepository<IUserXRoleRepository>();
    public IProfessionRepository ProfessionRepository => GetRepository<IProfessionRepository>();
    public ISpecialtyRepository SpecialtyRepository => GetRepository<ISpecialtyRepository>();
    public ISemesterRepository SemesterRepository => GetRepository<ISemesterRepository>();
    public ITopicVersionRepository TopicVersionRepository => GetRepository<ITopicVersionRepository>();
    public IStageIdeaRepositoty StageIdeaRepository => GetRepository<IStageIdeaRepositoty>();
    public IMentorTopicRequestRepository MentorTopicRequestRepository => GetRepository<IMentorTopicRequestRepository>();
    public ICapstoneScheduleRepository CapstoneScheduleRepository => GetRepository<ICapstoneScheduleRepository>();
    public ITimelineRepository TimelineRepository => GetRepository<ITimelineRepository>();
    public ICriteriaRepository CriteriaRepository => GetRepository<ICriteriaRepository>();
    public ICriteriaFormRepository CriteriaFormRepository => GetRepository<ICriteriaFormRepository>();
    public ICriteriaXCriteriaFormRepository CriteriaXCriteriaFormRepository => GetRepository<ICriteriaXCriteriaFormRepository>();
    public IAnswerCriteriaRepository AnswerCriteriaRepository => GetRepository<IAnswerCriteriaRepository>();

    public ITopicRepository TopicRepository => GetRepository<ITopicRepository>();
    public ITopicVersionRequestRepository TopicVersionRequestRepository => GetRepository<ITopicVersionRequestRepository>();
    public IIdeaVersionRepository IdeaVersionRepository => GetRepository<IIdeaVersionRepository>();

    public IMentorFeedbackRepository MentorFeedbackRepository => GetRepository<IMentorFeedbackRepository>();
}