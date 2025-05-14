using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

public class TopicRequestGetAllCurrentByStatus : GetQueryableQuery
{
    public TopicRequestStatus Status { get; set; }
}