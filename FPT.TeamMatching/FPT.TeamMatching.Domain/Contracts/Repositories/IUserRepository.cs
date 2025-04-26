using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Users;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetUserByUsernameOrEmail(string key);
    Task<User?> GetByEmail(string keyword);
    Task<User?> GetByUsername(string username);
    Task<List<PartnerInfoResult>> GetAllUsersWithNameOnly();
    Task<List<User>> GetCouncilsForIdeaVersionRequest(Guid ideaVersionId);
    Task<User?> GetReviewerByMatchingEmail(string keyword);
    Task<List<UserIdEmailResult>> GetAllReviewerIdAndUsername();

    Task<User?> GetById(Guid id);

    Task<List<User>?> GetStudentDoNotHaveTeam();

    Task<User?> GetByIdWithProjects(Guid? userId);

    Task<(List<User>, int)> GetAllByCouncilWithIdeaVersionRequestPending(UserGetAllQuery query);
    Task<List<EmailSuggestionModels>> GetAllEmailSuggestions(string email);
}