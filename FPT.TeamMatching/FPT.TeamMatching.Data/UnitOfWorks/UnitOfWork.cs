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
    public ITaskRepository TaskRepository => GetRepository<ITaskRepository>();
    public IReportRepository ReportRepository => GetRepository<IReportRepository>();
    public IInvitationUserRepository InvitationUserRepository => GetRepository<IInvitationUserRepository>();
    public ILecturerFeedbackRepository LecturerFeedbackRepository => GetRepository<ILecturerFeedbackRepository>();
    public IProjectRepository ProjectRepository => GetRepository<IProjectRepository>();
    public IProjectActivityRepository ProjectActivityRepository => GetRepository<IProjectActivityRepository>();
}