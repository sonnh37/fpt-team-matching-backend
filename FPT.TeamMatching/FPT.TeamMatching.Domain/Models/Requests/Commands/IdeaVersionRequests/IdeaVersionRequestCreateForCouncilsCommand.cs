using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaVersionRequests;

public class IdeaVersionRequestCreateForCouncilsCommand : CreateCommand
{
    public Guid? IdeaVersionId { get; set; }
}