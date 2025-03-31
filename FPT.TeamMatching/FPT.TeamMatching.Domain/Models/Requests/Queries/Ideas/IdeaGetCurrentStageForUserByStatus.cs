using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Ideas;

public class IdeaGetCurrentStageForUserByStatus
{
    public IdeaStatus? Status { get; set; }
}