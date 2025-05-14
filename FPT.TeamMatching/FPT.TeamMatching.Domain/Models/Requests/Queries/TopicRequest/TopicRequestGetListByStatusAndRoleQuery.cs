using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

public class TopicRequestGetListByStatusAndRoleQuery : GetQueryableQuery
{
    public TopicRequestStatus Status { get; set; }
    public TopicStatus TopicStatus { get; set; }

    public List<string> Roles { get; set; } = new List<string>();
}