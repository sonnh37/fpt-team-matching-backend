using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByUsernameOrEmail(string key);
    Task<User?> GetByEmail(string keyword);
    Task<User?> GetByUsername(string username);
    Task<List<PartnerInfoResult>> GetAllUsersWithNameOnly();
    Task<List<User>> GetThreeCouncilsForIdeaRequest();
    Task<User?> GetReviewerByMatchingEmail(string keyword);
}