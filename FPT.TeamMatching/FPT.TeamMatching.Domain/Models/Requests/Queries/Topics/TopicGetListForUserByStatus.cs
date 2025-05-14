using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

public class TopicGetListForUserByStatus : GetQueryableQuery
{
    public TopicStatus? Status { get; set; }
}