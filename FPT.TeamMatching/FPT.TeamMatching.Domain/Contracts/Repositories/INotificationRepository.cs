using FPT.TeamMatching.Domain.Contracts.Repositories.Bases;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;

namespace FPT.TeamMatching.Domain.Contracts.Repositories;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<List<Notification>> GetAllNotificationByUserId(Guid userId);

    Task<(List<Notification>, int)> GetDataByCurrentUser(NotificationGetAllByCurrentUserQuery query, Guid userId, Guid? projectId);
}