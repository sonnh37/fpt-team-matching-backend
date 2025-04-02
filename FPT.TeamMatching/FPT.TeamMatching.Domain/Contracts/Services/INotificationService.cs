using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface INotificationService : IBaseService
{
    Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification);
    Task<BusinessResult> GetNotificationByUserId(Guid userId);
    Task<BusinessResult> UpdateSeenNotification(Guid notificationId);
    Task<BusinessResult> DeleteNotification(Guid notificationId);
    Task<BusinessResult> GetNotificationsByCurrentUser(NotificationGetAllByCurrentUserQuery query); 
    Task<BusinessResult> CreateOrUpdate(CreateOrUpdateCommand createOrUpdateCommand);
}