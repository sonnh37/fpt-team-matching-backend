using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

public class IdeaVersionRequestGetListByStatusAndRoleQuery : GetQueryableQuery
{
    public IdeaVersionRequestStatus Status { get; set; }

    public List<string> Roles { get; set; } = new List<string>();

    public int? StageNumber { get; set; }

}