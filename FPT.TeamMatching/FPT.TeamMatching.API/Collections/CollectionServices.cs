using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Services;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionServices
{
    public static void AddCollectionServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<ITaskService, TaskService>();
        services.AddTransient<IReportService, ReportService>();
        services.AddTransient<IProjectService, ProjectService>();
        services.AddTransient<IProjectActivityService, ProjectActivityService>();
        services.AddTransient<ILecturerFeedbackService, LecturerFeedbackService>();
        services.AddTransient<IInvitationUserService, InvitationUserService>();
        services.AddTransient<IRefreshTokenService, RefreshTokenService>();
    }
}