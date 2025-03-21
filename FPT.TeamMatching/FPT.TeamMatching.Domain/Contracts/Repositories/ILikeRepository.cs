using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ILikeRepository : IBaseRepository<Like>
{
    Task<bool> DeleteLikeByBlogId(Guid blogId, Guid userId);
}