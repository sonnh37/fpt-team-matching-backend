using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Notification;

public class NotificationGetAllQuery : BaseQuery
{
    public Guid? UserId { get; set; }

    public string? Description { get; set; }

    public string? Type { get; set; }
}