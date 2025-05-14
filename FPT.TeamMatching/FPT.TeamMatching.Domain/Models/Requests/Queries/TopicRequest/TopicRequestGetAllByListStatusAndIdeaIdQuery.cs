using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.TopicRequest;

public class TopicRequestGetAllByListStatusAndIdeaIdQuery : GetQueryableQuery
{
    public Guid? TopicId { get; set; }

    public List<TopicRequestStatus> StatusList { get; set; } = new List<TopicRequestStatus>();
}