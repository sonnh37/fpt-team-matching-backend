using FPT.TeamMatching.Domain.Enums;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Base;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.IdeaRequests;

public class IdeaRequestUpdateStatusCommand : UpdateCommand
{
    public IdeaRequestStatus? Status { get; set; }
    
    public string? Content { get; set; }
}