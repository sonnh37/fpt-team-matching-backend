using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using Task = FPT.TeamMatching.Domain.Entities.Task;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<Notification>> GetAllNotificationByUserId(Guid userId);
}