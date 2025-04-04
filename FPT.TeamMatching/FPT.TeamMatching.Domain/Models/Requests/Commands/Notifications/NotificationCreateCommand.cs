using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;

public class NotificationCreateCommand : CreateCommand
{
    public Guid? UserId { get; set; }

    public Guid? ProjectId { get; set; }

    public string? Description { get; set; }

    public NotificationType? Type { get; set; }
    
    public bool? IsRead { get; set; }
}