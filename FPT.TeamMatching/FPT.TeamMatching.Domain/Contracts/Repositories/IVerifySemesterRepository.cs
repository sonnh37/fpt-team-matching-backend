using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface IVerifySemesterRepository : IBaseRepository<VerifySemester>
{
    Task<VerifySemester> FindByUserId(Guid semesterId);
}