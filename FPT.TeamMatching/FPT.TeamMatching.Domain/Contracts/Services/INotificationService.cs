using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Responses;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface INotificationService
{
    Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification);
    Task<BusinessResult> GetNotificationByUserId(Guid userId);
    Task<BusinessResult> UpdateSeenNotification(Guid notificationId);
    Task<BusinessResult> DeleteNotification(Guid notificationId);
}