using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notification;
using FPT.TeamMatching.Domain.Models.Responses;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = System.Threading.Tasks.Task;

namespace FPT.TeamMatching.Domain.Contracts.Services;

public interface INotificationService
{
    Task<BusinessResult> GenerateNotification(NotificationCreateCommand notification);
    Task<BusinessResult> GetNotificationByUserId(Guid userId);
    Task<BusinessResult> UpdateSeenNotification(Guid notificationId);
}