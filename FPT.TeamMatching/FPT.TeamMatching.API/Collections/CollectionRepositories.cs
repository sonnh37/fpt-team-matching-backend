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
    }
}