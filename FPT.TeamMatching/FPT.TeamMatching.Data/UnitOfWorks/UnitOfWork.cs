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
    public IApplicationRepository ApplicationRepository => GetRepository<IApplicationRepository>();
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
}