using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

public class TopicGetListForMentorQuery : GetQueryableQuery
{
    public List<string> Roles { get; set; } = new List<string>();
    public List<TopicStatus> Statuses { get; set; } = new List<TopicStatus>();
}