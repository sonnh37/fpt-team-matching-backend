using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;

public class IdeaRequestGetAllByListStatusForCurrentUser : GetQueryableQuery
{
    public List<IdeaRequestStatus> StatusList { get; set; } = new List<IdeaRequestStatus>();

}