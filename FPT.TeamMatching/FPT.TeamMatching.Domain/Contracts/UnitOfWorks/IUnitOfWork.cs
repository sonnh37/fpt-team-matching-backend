using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IUserRepository UserRepository { get; }
    IBlogRepository BlogRepository { get; }
    ILikeRepository LikeRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRateRepository RateRepository { get; }
    IIdeaRepository IdeaRepository { get; }
    IIdeaReviewRepository IdeaReviewRepository { get; }
    IApplicationRepository ApplicationRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    IProjectRepository ProjectRepository { get; }
    IFeedbackRepository FeedbackRepository { get; }
    IReviewRepository ReviewRepository { get; }
    IInvitationRepository InvitationRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    INotificationRepository NotificationRepository { get; }
    IProfileStudentRepository ProfileStudentRepository { get; }
    ISkillProfileRepository SkillProfileRepository { get; }
    IRoleRepository RoleRepository { get; }
    IUserXRoleRepository UserXRoleRepository { get; }
}