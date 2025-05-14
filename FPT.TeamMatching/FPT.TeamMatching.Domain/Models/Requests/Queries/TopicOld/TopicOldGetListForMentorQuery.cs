using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicOld;

public class TopicOldGetListForMentorQuery : GetQueryableQuery
{
    public List<string> Roles { get; set; } = new List<string>();
}