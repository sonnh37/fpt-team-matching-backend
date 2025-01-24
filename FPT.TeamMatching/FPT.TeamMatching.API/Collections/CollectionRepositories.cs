using FPT.TeamMatching.Data.Repositories;
using FPT.TeamMatching.Data.Repositories.Base;
using FPT.TeamMatching.Data.UnitOfWorks;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionRepositories
{
    public static void AddCollectionRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectActivityRepository, ProjectActivityRepository>();
        services.AddScoped<ILecturerFeedbackRepository, LecturerFeedbackRepository>();
        services.AddScoped<IInvitationUserRepository, InvitationUserRepository>();
    }
}