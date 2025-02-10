using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface ITaskRepository : IBaseRepository<Task>
{
}