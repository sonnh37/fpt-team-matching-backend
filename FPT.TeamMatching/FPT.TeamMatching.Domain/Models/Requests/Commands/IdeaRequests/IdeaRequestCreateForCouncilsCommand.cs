using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;

public class IdeaRequestCreateForCouncilsCommand : CreateCommand
{
    public Guid? IdeaId { get; set; }
}