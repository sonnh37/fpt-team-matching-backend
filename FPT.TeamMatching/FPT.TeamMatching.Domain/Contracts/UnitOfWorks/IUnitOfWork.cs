using FPT.TeamMatching.Domain.Contracts.Repositories;

namespace FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IUserRepository UserRepository { get; }

    IProjectRepository ProjectRepository { get; }

    IProjectActivityRepository ProjectActivityRepository { get; }

    ILecturerFeedbackRepository LecturerFeedbackRepository { get; }

    ITaskRepository TaskRepository { get; }

    IReportRepository ReportRepository { get; }

    IInvitationUserRepository InvitationUserRepository { get; }
    
    IRefreshTokenRepository RefreshTokenRepository { get; }
}