using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

public class IdeaGetCurrentByStatus : GetQueryableQuery
{
    public IdeaStatus? Status { get; set; }
}