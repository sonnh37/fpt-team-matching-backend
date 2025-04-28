using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

public class IdeaGetListByStatusAndRoleQuery : GetQueryableQuery
{
    public IdeaVersionRequestStatus Status { get; set; }
    public IdeaStatus IdeaStatus { get; set; }

    public List<string> Roles { get; set; } = new List<string>();
}