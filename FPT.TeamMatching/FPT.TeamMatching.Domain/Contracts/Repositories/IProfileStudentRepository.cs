using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IProfileStudentRepository : IBaseRepository<ProfileStudent>
{
    Task<ProfileStudent?> GetProfileByUserId(Guid userId);
    Task<List<ProfileStudent>> GetProfileByUserIds(List<Guid> guids);
}