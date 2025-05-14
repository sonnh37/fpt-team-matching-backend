using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.TopicRequests;

public class TopicRequestCreateForCouncilsCommand : CreateCommand
{
    public Guid? TopicId { get; set; }
}