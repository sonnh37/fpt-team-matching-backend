using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IProfileRepository : IBaseRepository<Profile>
{
    Task<Profile> GetProfileByUserId(Guid userId);
}