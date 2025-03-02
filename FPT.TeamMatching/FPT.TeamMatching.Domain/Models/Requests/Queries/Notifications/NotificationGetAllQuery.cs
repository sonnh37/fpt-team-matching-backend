using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;

public class NotificationGetAllQuery : GetQueryableQuery
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }
    
    public bool? IsRead { get; set; }
}