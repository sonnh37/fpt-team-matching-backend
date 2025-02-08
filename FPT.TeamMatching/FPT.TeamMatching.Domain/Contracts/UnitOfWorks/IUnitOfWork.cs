using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IUserRepository UserRepository { get; }
    IBlogRepository BlogRepository { get; }
    ILikeRepository LikeRepository { get; }
    ICommentRepository CommentRepository { get; }
    IRateRepository RateRepository { get; }
    IJobPositionRepository JobPositionRepository { get; }
    ITeamMemberRepository TeamMemberRepository { get; }
    IProjectRepository ProjectRepository { get; }

    IProjectActivityRepository ProjectActivityRepository { get; }

    ILecturerFeedbackRepository LecturerFeedbackRepository { get; }

    ITaskRepository TaskRepository { get; }

    IReportRepository ReportRepository { get; }

    IInvitationUserRepository InvitationUserRepository { get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    
    INotificationRepository NotificationRepository { get; }
    IProfileRepository ProfileRepository { get; }
    ISkillProfileRepository SkillProfileRepository { get; }
    IVerifyQualifiedForAcademicProjectRepository VerifyQualifiedForAcademicProjectRepository { get; }
    IVerifySemesterRepository VerifySemesterRepository { get; }
}