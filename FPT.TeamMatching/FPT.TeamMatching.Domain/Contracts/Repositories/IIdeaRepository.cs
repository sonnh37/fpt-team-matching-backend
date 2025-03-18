using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IIdeaRepository : IBaseRepository<Idea>
{
    Task<IList<Idea>> GetIdeasByUserId(Guid userId);
    
    Task<int> MaxNumberOfSemester(Guid semesterId);

    Task<List<Idea>> GetCurrentIdeaByUserIdAndStatus(Guid userId, IdeaStatus status);

    Task<Idea?> GetLatestIdeaByUserAndStatus(Guid userId, IdeaStatus status);
}