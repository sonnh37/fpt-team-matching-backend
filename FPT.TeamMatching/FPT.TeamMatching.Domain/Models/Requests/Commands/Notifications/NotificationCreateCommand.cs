using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;

public class NotificationCreateCommand : CreateCommand
{
    public Guid UserId { get; set; }

    public string Description { get; set; }

    public string Type { get; set; }
}