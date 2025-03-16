using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;

public class IdeaRequestGetAllCurrentByStatus : GetQueryableQuery
{
    public IdeaRequestStatus Status { get; set; }
}