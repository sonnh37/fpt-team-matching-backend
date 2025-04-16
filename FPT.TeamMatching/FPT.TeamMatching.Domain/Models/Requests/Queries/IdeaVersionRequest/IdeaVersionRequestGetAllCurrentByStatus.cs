using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

public class IdeaVersionRequestGetAllCurrentByStatus : GetQueryableQuery
{
    public IdeaVersionRequestStatus Status { get; set; }
}