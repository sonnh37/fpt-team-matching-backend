using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Services;

namespace FPT.TeamMatching.API.Collections;

public static class CollectionServices
{
    public static void AddCollectionServices(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IBlogService, BlogService>();
        services.AddTransient<ILikeService, LikeService>();
        services.AddTransient<ICommentService, CommentService>();
        services.AddTransient<IRateService, RateService>();
        services.AddTransient<IJobPositionService, JobPositionService>();
        services.AddTransient<ITeamMemberService, TeamMemberService>();
        
    }
}