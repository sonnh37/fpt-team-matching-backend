using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Ideas;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IIdeaRepository : IBaseRepository<Idea>
{
    Task<IList<Idea>?> GetIdeasByUserId(Guid userId);
}