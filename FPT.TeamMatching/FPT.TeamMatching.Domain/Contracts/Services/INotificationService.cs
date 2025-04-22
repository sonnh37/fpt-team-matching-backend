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
    //gửi noti cho list user
    Task<BusinessResult> CreateForGroupUsers(NotificationCreateForGroupUser createCommand, List<Guid> userIds);
    //gửi noti cho invidual
    Task<BusinessResult> CreateForUser(NotificationCreateForIndividual createCommand);
    //gửi noti cho team
    Task<BusinessResult> CreateForTeam(NotificationCreateForTeam createCommand);
    //gửi noti cho all system
    Task<BusinessResult> CreateForSystemWide(NotificationCreateForSystemWide createCommand);
    //gửi noti cho role based
    Task<BusinessResult> CreateForRoleBased(NotificationCreateForRoleBased createCommand);
    Task<BusinessResult> CreateForIndividual(NotificationCreateForIndividual createCommand);
    Task<BusinessResult> MarkAsReadAsync(Guid id);
    Task<BusinessResult> MarkAllAsReadAsync();
}