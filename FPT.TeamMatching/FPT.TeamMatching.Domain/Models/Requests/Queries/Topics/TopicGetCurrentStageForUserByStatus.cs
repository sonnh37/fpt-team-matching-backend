using FPT.TeamMatching.Domain.Enums;

namespace FPT.TeamMatching.Domain.Models.Requests.Queries.Topics;

public class TopicGetCurrentStageForUserByStatus
{
    public TopicStatus? Status { get; set; }
}