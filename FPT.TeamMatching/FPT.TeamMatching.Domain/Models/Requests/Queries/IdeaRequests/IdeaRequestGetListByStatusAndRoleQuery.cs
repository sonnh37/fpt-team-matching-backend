using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaRequests;

public class IdeaRequestGetListByStatusAndRoleQuery : GetQueryableQuery
{
    public IdeaRequestStatus Status { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
    
    public int? StageNumber { get; set; }
    
}