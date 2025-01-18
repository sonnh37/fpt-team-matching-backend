using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    // Task<User?> FindUsernameOrEmail(string key);
    // Task<User?> GetByEmail(string keyword);
    // Task<User?> GetByUsername(string username);
}