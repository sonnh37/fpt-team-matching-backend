using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<Notification>> GetAllNotificationByUserId(Guid userId);
}