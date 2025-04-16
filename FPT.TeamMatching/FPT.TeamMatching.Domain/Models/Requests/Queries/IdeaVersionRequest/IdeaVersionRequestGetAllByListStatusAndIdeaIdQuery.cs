using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Queries.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.IdeaVersionRequest;

public class IdeaVersionRequestGetAllByListStatusAndIdeaIdQuery : GetQueryableQuery
{
    public Guid? IdeaId { get; set; }

    public List<IdeaVersionRequestStatus> StatusList { get; set; } = new List<IdeaVersionRequestStatus>();
}