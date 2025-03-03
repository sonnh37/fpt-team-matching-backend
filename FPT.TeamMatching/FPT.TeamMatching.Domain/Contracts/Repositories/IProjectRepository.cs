using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<Project?> GetProjectByUserIdLogin(Guid userId);
}