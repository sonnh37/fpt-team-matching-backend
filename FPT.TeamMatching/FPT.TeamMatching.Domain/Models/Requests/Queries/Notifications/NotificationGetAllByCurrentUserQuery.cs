using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Notifications;

public class NotificationGetAllByCurrentUserQuery : GetQueryableQuery
{
    public bool? IsRead { get; set; }
}