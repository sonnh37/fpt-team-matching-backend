using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

public class TopicGetListForMentorQuery : GetQueryableQuery
{
    public List<string> Roles { get; set; } = new List<string>();
}