using FPT.TeamMatching.Domain.Contracts.Services.Bases;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Models.Results.Bases;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface INotificationService : IBaseService
{
    Task<BusinessResult> GetNotificationByUserId(Guid userId);
    Task<BusinessResult> UpdateSeenNotification(Guid notificationId);
    Task<BusinessResult> DeleteNotification(Guid notificationId);
    Task<BusinessResult> GetNotificationsByCurrentUser(NotificationGetAllByCurrentUserQuery query); 
    Task<BusinessResult> CreateOrUpdate(CreateOrUpdateCommand createOrUpdateCommand);
    //gửi noti cho 1 user
    Task<BusinessResult> CreateForUser(NotificationCreateCommand createCommand);
    //gửi noti cho list user
    Task<BusinessResult> CreateForGroup(NotificationCreateForGroup createCommand, List<Guid> userIds);
    //gửi noti cho team
    Task<BusinessResult> CreateForTeam(NotificationCreateForGroup createCommand, Guid projectId);
}